//
// Mono.Cxxi.CppInstancePtr.cs: Represents a pointer to a native C++ instance
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
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

using Mono.Cxxi.Abi;

namespace Mono.Cxxi {
	public struct CppInstancePtr : ICppObject {

		private IntPtr ptr, native_vtptr;
		private bool manage_memory;
        private Dictionary<Type, IntPtr> base_vtables;
        private Dictionary<Type, IntPtr> base_ptrs;

		private static Dictionary<IntPtr,int> managed_vtptr_to_gchandle_offset = null;

		// Alloc a new C++ instance
		internal CppInstancePtr (CppTypeInfo typeInfo, object managedWrapper)
		{
			// Under the hood, we're secretly subclassing this C++ class to store a
			// handle to the managed wrapper.
			int allocSize = typeInfo.GCHandleOffset + IntPtr.Size;
			ptr = Marshal.AllocHGlobal (allocSize);

			// NOTE: native_vtptr will be set later after native ctor is called
			native_vtptr = IntPtr.Zero;
            base_vtables = new Dictionary<Type, IntPtr>();
            base_ptrs = new Dictionary<Type, IntPtr>();

			// zero memory for sanity
			// FIXME: This should be an initblk
			byte[] zeroArray = new byte [allocSize];
			Marshal.Copy (zeroArray, 0, ptr, allocSize);

			IntPtr handlePtr = MakeGCHandle (managedWrapper);
			Marshal.WriteIntPtr (ptr, typeInfo.GCHandleOffset, handlePtr);

			manage_memory = true;
		}

		// Alloc a new C++ instance when there is no managed wrapper.
		internal CppInstancePtr (int nativeSize)
		{
			ptr = Marshal.AllocHGlobal (nativeSize);
			native_vtptr = IntPtr.Zero;
            base_vtables = new Dictionary<Type, IntPtr>();
            base_ptrs = new Dictionary<Type, IntPtr>();
			manage_memory = true;
		}

		// Gets a casted CppInstancePtr
		internal CppInstancePtr (CppInstancePtr instance, int offset)
		{
			// FIXME: On NET_4_0 use IntPtr.Add
			ptr = new IntPtr (instance.Native.ToInt64 () + offset);
			native_vtptr = IntPtr.Zero;
            base_vtables = new Dictionary<Type, IntPtr>(instance.base_vtables);
            base_ptrs = new Dictionary<Type, IntPtr>(instance.base_ptrs);
			manage_memory = false;
		}

		// Get a CppInstancePtr for an existing C++ instance from an IntPtr
		public CppInstancePtr (IntPtr native)
		{
			if (native == IntPtr.Zero)
				throw new ArgumentOutOfRangeException ("native cannot be null pointer");

			ptr = native;
			native_vtptr = IntPtr.Zero;
            base_vtables = new Dictionary<Type, IntPtr>();
            base_ptrs = new Dictionary<Type, IntPtr>();
			manage_memory = false;
		}

		// Fulfills ICppObject requirement
		public CppInstancePtr (CppInstancePtr copy)
		{
			this.ptr = copy.ptr;
			this.native_vtptr = copy.native_vtptr;
			this.manage_memory = copy.manage_memory;
            this.base_vtables = new Dictionary<Type, IntPtr>(copy.base_vtables);
            this.base_ptrs = new Dictionary<Type, IntPtr>(copy.base_ptrs);
		}

		// Provide casts to/from IntPtr:
		public static implicit operator CppInstancePtr (IntPtr native)
		{
			return new CppInstancePtr (native);
		}

		// cast from CppInstancePtr -> IntPtr is explicit because we lose information
		public static explicit operator IntPtr (CppInstancePtr ip)
		{
			return ip.Native;
		}

		public IntPtr Native {
			get {
				if (ptr == IntPtr.Zero)
					throw new ObjectDisposedException ("CppInstancePtr");

				return ptr;
			}
		}

		// Internal for now to prevent attempts to read vtptr from non-virtual class
		internal IntPtr NativeVTable {
			get {

				if (native_vtptr == IntPtr.Zero) {
					// For pointers from native code...
					// Kludge! CppInstancePtr doesn't know whether this class is virtual or not, but we'll just assume that either
					//  way it's at least sizeof(void*) and read what would be the vtptr anyway. Supposedly, if it's not virtual,
					//  the wrappers won't use this field anyway...
					native_vtptr = Marshal.ReadIntPtr (ptr);
				}

				return native_vtptr;
			}
			set {
				native_vtptr = value;
			}
		}

        internal IntPtr GetNativeBaseVTable(Type t)
        {
            IntPtr value = IntPtr.Zero;
            base_vtables.TryGetValue(t, out value);
            return value;
        }

        internal void SetNativeBaseVTable(Type t, IntPtr ptr)
        {
            if (base_vtables.ContainsKey(t))
                base_vtables[t] = ptr;
            else
                base_vtables.Add(t, ptr);
        }

        internal IntPtr GetNativeBasePointer(Type t)
        {
            IntPtr value = IntPtr.Zero;
            base_ptrs.TryGetValue(t, out value);
            return value;
        }

        internal void SetNativeBasePointer(Type t, IntPtr ptr)
        {
            if (base_ptrs.ContainsKey(t))
                base_ptrs[t] = ptr;
            else
                base_ptrs.Add(t, ptr);
        }

		CppInstancePtr ICppObject.Native {
			get { return this; }
		}

		public bool IsManagedAlloc {
			get { return manage_memory; }
		}

		internal static void RegisterManagedVTable (VTable vtable)
		{
			if (managed_vtptr_to_gchandle_offset == null)
				managed_vtptr_to_gchandle_offset = new Dictionary<IntPtr, int> ();

			managed_vtptr_to_gchandle_offset [vtable.Pointer] = vtable.TypeInfo.GCHandleOffset;
		}

		internal static IntPtr MakeGCHandle (object managedWrapper)
		{
			// TODO: Dispose() should probably be called at some point on this GCHandle.
			GCHandle handle = GCHandle.Alloc (managedWrapper, GCHandleType.Normal);
			return GCHandle.ToIntPtr (handle);
		}

		// This might be made public, but in this form it only works for classes with vtables.
		//  Returns null if the native ptr passed in does not appear to be a managed instance.
		//  (i.e. its vtable ptr is not in managed_vtptr_to_gchandle_offset)
		internal static T ToManaged<T> (IntPtr native) where T : class
		{
			if (managed_vtptr_to_gchandle_offset == null)
				return null;

			int gchOffset;
			if (!managed_vtptr_to_gchandle_offset.TryGetValue (Marshal.ReadIntPtr (native), out gchOffset))
				return null;

			return ToManaged<T> (native, gchOffset);
		}

		// WARNING! This method is not safe. DO NOT call
		// if we do not KNOW that this instance is managed.
		internal static T ToManaged<T> (IntPtr native, int nativeSize) where T : class
		{
			IntPtr handlePtr = Marshal.ReadIntPtr (native, nativeSize);
			GCHandle handle = GCHandle.FromIntPtr (handlePtr);

			return handle.Target as T;
		}

		// TODO: Free GCHandle?
		public void Dispose ()
		{
			if (manage_memory && ptr != IntPtr.Zero)
				Marshal.FreeHGlobal (ptr);

			ptr = IntPtr.Zero;
			manage_memory = false;
		}
	}
}
