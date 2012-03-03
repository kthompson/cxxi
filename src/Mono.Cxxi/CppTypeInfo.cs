//
// Mono.Cxxi.CppTypeInfo.cs: Type metadata for C++ types
//
// Author:
//   Alexander Corrado (alexander.corrado@gmail.com)
//   Andreia Gaita (shana@spoiledcat.net)
//
// Copyright (C) 2010-2011 Alexander Corrado
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Runtime.InteropServices;

using Mono.Cxxi.Abi;
using Mono.Cxxi.Util;

namespace Mono.Cxxi {

	public enum BaseVirtualMethods {

		// Prepends this base's virtual methods to the primary vtable
		PrependPrimary,

		// Appends this base's virtual methods to the primary vtable
		AppendPrimary,

		// Creates a new out-of-band vtable for this base's virtual methods
		NewVTable,

	}

	// NOTE: As AddBase is called, properties change.
	//  TypeComplete indicates when the dust has settled.
	public class CppTypeInfo {

		public CppLibrary Library { get; private set; }

		public string TypeName { get; private set; }
		public bool IsPrimaryBase { get; protected set; } // < True by default. set to False in cases where it is cloned as a non-primary base
        public bool IsFinalClass { get; protected set; } // < True by default. set to False when added as a base class of another class

		// returns the number of vtable slots reserved for the
		//  base class(es) before this class's virtual methods start
		public int BaseVTableSlots { get; protected set; }

		public Type InterfaceType { get; private set; }
		public Type NativeLayout { get; private set; }
		public Type WrapperType { get; private set; }

		// read only versions:
		public IList<PInvokeSignature> VirtualMethods { get; private set; }
		public IList<Type> VTableDelegateTypes { get; private set; }
		public IList<Delegate> VTableOverrides { get; private set; }
		public IList<CppTypeInfo> BaseClasses { get; private set; }

		// backing lists:
		protected List<PInvokeSignature> virtual_methods;
		protected LazyGeneratedList<Type> vt_delegate_types;
		protected LazyGeneratedList<Delegate> vt_overrides;
		protected List<CppTypeInfo> base_classes;

		protected List<int> vtable_index_adjustments;

		protected int native_size_without_padding; // <- this refers to the size of all the fields declared in the nativeLayout struct
		protected int field_offset_padding_without_vtptr;
		protected int gchandle_offset_delta;

		private VTable lazy_vtable;

		internal EmitInfo emit_info; // <- will be null when the type is done being emitted
	    protected internal bool? HasNonDefaultCopyCtorOrDtor;
	    public bool TypeComplete { get { return emit_info == null; } }

		#region Construction

		public CppTypeInfo (CppLibrary lib, string typeName, Type interfaceType, Type nativeLayout, Type/*?*/ wrapperType)
			: this ()
		{
			Library = lib;
			TypeName = typeName;

			InterfaceType = interfaceType;
			NativeLayout = nativeLayout;
			WrapperType = wrapperType;

			virtual_methods = new List<PInvokeSignature> (Library.Abi.GetVirtualMethodSlots (this, interfaceType));
			VirtualMethods = new ReadOnlyCollection<PInvokeSignature> (virtual_methods);

			vt_delegate_types = new LazyGeneratedList<Type> (virtual_methods.Count, i => DelegateTypeCache.GetDelegateType (virtual_methods [i]));
			VTableDelegateTypes = new ReadOnlyCollection<Type> (vt_delegate_types);

			vt_overrides = new LazyGeneratedList<Delegate> (virtual_methods.Count, i => Library.Abi.GetManagedOverrideTrampoline (this, i));
			VTableOverrides = new ReadOnlyCollection<Delegate> (vt_overrides);

			vtable_index_adjustments = new List<int> (virtual_methods.Count);

			if (nativeLayout != null)
				native_size_without_padding = nativeLayout.GetFields ().Any ()? Marshal.SizeOf (nativeLayout) : 0;
		}

		protected CppTypeInfo ()
		{
			base_classes = new List<CppTypeInfo> ();
			BaseClasses = new ReadOnlyCollection<CppTypeInfo> (base_classes);

			field_offset_padding_without_vtptr = 0;
			gchandle_offset_delta = 0;
			IsPrimaryBase = true;
            IsFinalClass = true;
			BaseVTableSlots = 0;
			lazy_vtable = null;

			emit_info = new EmitInfo ();
		}

		// The contract for Clone is that, if TypeComplete, working with the clone *through the public
		//  interface* is guaranteed not to affect the original. Note that any subclassses
		// have access to protected stuff that is not covered by this guarantee.
		public virtual CppTypeInfo Clone ()
		{
			return this.MemberwiseClone () as CppTypeInfo;
		}

		#endregion

		#region Type Layout

		public virtual int Alignment {
			get { return IntPtr.Size; }
		}

		// the extra padding to allocate at the top of the class before the fields begin
		//  (by default, just the vtable pointer)
		public virtual int FieldOffsetPadding {
			get { return field_offset_padding_without_vtptr +
                (this.HasVTable ? IntPtr.Size : 0) +
                (this.HasVBTable ? IntPtr.Size : 0); }
		}

		public virtual int NativeSize {
			get {
				var basesize = native_size_without_padding + FieldOffsetPadding;
				return basesize + (basesize % Alignment);
			}
		}

		public virtual int GCHandleOffset {
			get { return NativeSize + gchandle_offset_delta; }
		}


		public void AddBase (CppTypeInfo baseType)
		{
			// by default, only the primary base shares the subclass's primary vtable
			AddBase (baseType, base_classes.Count == 0 ? BaseVirtualMethods.PrependPrimary : BaseVirtualMethods.NewVTable);
		}

		protected virtual void AddBase (CppTypeInfo baseType, BaseVirtualMethods location)
		{
			if (TypeComplete)
				return;

			var baseVMethodCount = baseType.virtual_methods.Count;
			baseType = baseType.Clone ();
            baseType.IsFinalClass = false;

            // Since the class completes before getting added to the parent
            // We need to remove the size we added for any virtual bases
            foreach (var virt in baseType.GetVirtualBasesDistinct())
            {
                baseType.field_offset_padding_without_vtptr -= virt.native_size_without_padding +
                    virt.FieldOffsetPadding;
            }

            // Don't add virtual methods of virtual bases because they're kept in their own VTable
            bool isVirtualBase = VirtualBaseAttribute.IsVirtualBaseOf(this.WrapperType, baseType.WrapperType);
            if (!isVirtualBase)
            {
                switch (location)
                {

                    case BaseVirtualMethods.PrependPrimary:

                        for (int i = 0; i < baseVMethodCount; i++)
                            virtual_methods.Insert(BaseVTableSlots + i, baseType.virtual_methods[i]);

                        gchandle_offset_delta = baseType.gchandle_offset_delta;

                        BaseVTableSlots += baseVMethodCount;
                        vt_delegate_types.Add(baseVMethodCount);
                        vt_overrides.Add(baseVMethodCount);
                        break;

                    case BaseVirtualMethods.AppendPrimary:

                        for (int i = 0; i < baseVMethodCount; i++)
                            virtual_methods.Add(baseType.virtual_methods[i]);

                        gchandle_offset_delta = baseType.gchandle_offset_delta;

                        vt_delegate_types.Add(baseVMethodCount);
                        vt_overrides.Add(baseVMethodCount);
                        break;

                    case BaseVirtualMethods.NewVTable:

                        baseType.IsPrimaryBase = (base_classes.Count == 0);

                        // offset all previously added bases
                        foreach (var previousBase in base_classes)
                            previousBase.gchandle_offset_delta += baseType.NativeSize;

                        // offset derived (this) type's gchandle
                        gchandle_offset_delta += baseType.gchandle_offset_delta + IntPtr.Size;

                        baseType.gchandle_offset_delta += CountBases(b => !b.IsPrimaryBase) * IntPtr.Size;

                        // ensure managed override tramps will be regenerated with correct gchandle offset
                        baseType.vt_overrides = new LazyGeneratedList<Delegate>(baseType.virtual_methods.Count, i => Library.Abi.GetManagedOverrideTrampoline(baseType, i));
                        baseType.VTableOverrides = new ReadOnlyCollection<Delegate>(baseType.vt_overrides);
                        baseType.lazy_vtable = null;
                        break;
                }

                field_offset_padding_without_vtptr += baseType.native_size_without_padding +
                    (location == BaseVirtualMethods.NewVTable ?
                    baseType.FieldOffsetPadding : baseType.field_offset_padding_without_vtptr +
                    (baseType.HasVBTable ? IntPtr.Size : 0));
            }

			base_classes.Add (baseType);
		}

		public virtual void CompleteType ()
		{
			if (emit_info == null)
				return;

			foreach (var baseClass in base_classes)
				baseClass.CompleteType ();

            // Update the offsets for virtual base classes
            if (this.IsFinalClass)
            {
                // TODO: This GCHandleOffset stuff probably needs a bit of work
                // I think the GCHandleOffset is typically only used on the final class
                // and for non primary base classes in which case the offsets should be right
                // However, I wouldn't trust the values for any other types

                int offset = 0;
                var virtDict = new Dictionary<Type, int>();
                foreach (var virt in this.GetVirtualBases().Reverse())
                {
                    // If we've already seen this type then use the offset from before
                    if (virtDict.ContainsKey(virt.WrapperType))
                    {
                        virt.gchandle_offset_delta = virtDict[virt.WrapperType];
                        continue;
                    }

                    virtDict.Add(virt.WrapperType, offset);
                    virt.gchandle_offset_delta = offset;
                    offset += virt.NativeSize;
                    this.field_offset_padding_without_vtptr += virt.native_size_without_padding +
                        virt.FieldOffsetPadding;
                }

                // If we have any virtual bases then we need to offset by their sizes
                if (virtDict.Count > 0)
                {
                    EachBase((b) =>
                        {
                            // Don't offset any of our virtual bases because they're already done
                            if (virtDict.ContainsKey(b.WrapperType))
                                return;

                            b.gchandle_offset_delta += offset;
                        });
                }
            }

			emit_info = null;

			RemoveVTableDuplicates ();
		}

		public virtual CppType GetMangleType ()
		{
			var mangleType = Library.Abi.GetMangleType (InterfaceType, InterfaceType);
			mangleType.ElementTypeName = this.WrapperType.GetCustomAttributes(typeof(TemplateClassAttribute), false)
                .Cast<TemplateClassAttribute>().Select(a => a.ClassName).FirstOrDefault() ?? this.TypeName;
			return mangleType;
		}

        public void EachBase (Action<CppTypeInfo> predicate)
        {
            foreach (var b in base_classes)
            {
                b.EachBase(predicate);
                predicate(b);
            }
        }

		public int CountBases (Func<CppTypeInfo, bool> predicate)
		{
			int count = 0;
			foreach (var baseClass in base_classes) {
				count += baseClass.CountBases (predicate);
				count += predicate (baseClass)? 1 : 0;
			}
			return count;
		}

		#endregion

		#region Casting

		protected virtual CppTypeInfo GetCastInfo (Type sourceType, Type targetType, out int offset)
		{
			offset = 0;

			if (WrapperType.Equals (targetType)) {
				// check for downcast (base type -> this type)

				foreach (var baseClass in base_classes) {
					if (baseClass.WrapperType.Equals (sourceType)) {
						return baseClass;
					}
					offset -= baseClass.NativeSize;
				}


			} else if (WrapperType.IsAssignableFrom (sourceType)) {
				// check for upcast (this type -> base type)

				foreach (var baseClass in base_classes) {
					if (baseClass.WrapperType.Equals (targetType)) {
						return baseClass;
					}
					offset += baseClass.NativeSize;
				}

			} else {
				throw new ArgumentException ("Either source type or target type must be equal to this wrapper type.");
			}

			throw new InvalidCastException ("Cannot cast an instance of " + sourceType + " to " + targetType);
		}

		public virtual CppInstancePtr Cast (ICppObject instance, Type targetType)
		{
			int offset;
			var baseTypeInfo = GetCastInfo (instance.GetType (), targetType, out offset);
			var result = new CppInstancePtr (instance.Native, offset);

			if (offset > 0 && instance.Native.IsManagedAlloc && baseTypeInfo.HasVTable) {
				// we might need to paste the managed base-in-derived vtptr here --also inits native_vtptr
                baseTypeInfo.VTable.InitInstanceOffset(ref result, false, 0);

                // Make sure we update the vtable location in the original instance as well
                instance.Native.SetNativeBaseVTable(baseTypeInfo.WrapperType, result.NativeVTable);
			}

			return result;
		}

		public virtual TTarget Cast<TTarget> (ICppObject instance) where TTarget : class
		{
			TTarget result;
			var ptr = Cast (instance, typeof (TTarget));

			// Check for existing instance based on vtable ptr
			result = CppInstancePtr.ToManaged<TTarget> (ptr.Native);

			// Create a new wrapper if necessary
			if (result == null)
				result = Activator.CreateInstance (typeof (TTarget), ptr) as TTarget;

			return result;
		}

		public virtual void InitNonPrimaryBase (ICppObject baseInDerived, ICppObject derived, Type baseType)
		{
			int offset;
			var baseTypeInfo = GetCastInfo (derived.GetType (), baseType, out offset);
            Marshal.WriteIntPtr(baseInDerived.Native.Native, baseTypeInfo.GCHandleOffset, CppInstancePtr.MakeGCHandle(baseInDerived));
		}

		#endregion

		#region V-Table

		public virtual bool HasVTable {
			get { return VirtualMethods.Any (); }
		}

        public virtual bool HasVBTable {
            get { return false; }
        }

		public virtual VTable VTable {
			get {
				CompleteType ();
				if (!HasVTable && !this.GetVirtualBases().Any())
					return null;

				if (lazy_vtable == null)
					lazy_vtable = new VTable (this);

				return lazy_vtable;
			}
		}

		// the padding in the data pointed to by the vtable pointer before the list of function pointers starts
		public virtual int VTableTopPadding {
			get { return 0; }
		}

		// the amount of extra room alloc'd after the function pointer list of the vtbl
		public virtual int VTableBottomPadding {
			get { return 0; }
		}

        public virtual IEnumerable<CppTypeInfo> GetVirtualBasesDistinct()
        {
            var unique = new List<Type>();
            foreach (var b in this.GetVirtualBases())
            {
                if (unique.Contains(b.WrapperType))
                    continue;

                yield return b;
                unique.Add(b.WrapperType);
            }
        }

        public virtual IEnumerable<CppTypeInfo> GetVirtualBases()
        {
            foreach (var sub in BaseClasses)
            {
                foreach (var virt in sub.GetVirtualBases())
                    yield return virt;

                if (VirtualBaseAttribute.IsVirtualBaseOf(WrapperType, sub.WrapperType))
                    yield return sub;
            }
        }

        public virtual CppInstancePtr GetNativeVirtualPointer(CppInstancePtr instance)
        {
            // Determine if we should return a pointer to a virtual base or not
            var ptr = instance.GetNativeBasePointer(this.WrapperType);
            if (ptr == IntPtr.Zero)
                return instance;

            // Return a new instance pointer offset to the correct location
            return new CppInstancePtr(instance, (int)(ptr.ToInt64() - instance.Native.ToInt64()));
        }

		public virtual T GetAdjustedVirtualCall<T> (CppInstancePtr instance, int derivedVirtualMethodIndex)
			where T : class /* Delegate */
		{
			var base_adjusted = BaseVTableSlots + derivedVirtualMethodIndex;
			return VTable.GetVirtualCallDelegate<T> (instance, base_adjusted + vtable_index_adjustments [base_adjusted]);
		}

		protected virtual void RemoveVTableDuplicates ()
		{
			// check that any virtual methods overridden in a subclass are only included once
			var vsignatures = new Dictionary<MethodSignature,PInvokeSignature> (MethodSignature.EqualityComparer);
			var adjustment  = 0;

			for (int i = 0; i < virtual_methods.Count; i++) {
				vtable_index_adjustments.Add (adjustment);

				var sig = virtual_methods [i];
				if (sig == null)
					continue;

				PInvokeSignature existing;
				if (vsignatures.TryGetValue (sig, out existing)) {
					OnVTableDuplicate (ref i, ref adjustment, sig, existing);
				} else {
					vsignatures.Add (sig, sig);
				}
			}
		}

		protected virtual bool OnVTableDuplicate (ref int iter, ref int adj, PInvokeSignature sig, PInvokeSignature dup)
		{
			// This predicate ensures that duplicates are only removed
			// if declared in different classes (i.e. overridden methods).
			// We usually want to allow the same exact virtual methods to appear
			// multiple times, in the case of nonvirtual diamond inheritance, for example.
			if (!sig.OrigMethod.Equals (dup.OrigMethod)) {
				virtual_methods.RemoveAt (iter--);
				vt_overrides.Remove (1);
				vt_delegate_types.Remove (1);
				adj--;
				return true;
			}

			return false;
		}

		#endregion
	}

	// This is used internally by CppAbi:
	internal class DummyCppTypeInfo : CppTypeInfo {

		public CppTypeInfo BaseTypeInfo { get; set; }

		protected override void AddBase (CppTypeInfo baseType, BaseVirtualMethods location)
		{
			BaseTypeInfo = baseType;
		}
	}
}

