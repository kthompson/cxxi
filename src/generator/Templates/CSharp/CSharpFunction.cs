﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.530
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Templates {
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using Mono.Cxxi;
    using System;
    
    
    public partial class CSharpFunction : FunctionBase {
        
        
        #line 35 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"


private void WriteParameters (IList<Parameter> parameters)
{
	for (var i = 0; i < parameters.Count; i++) {
		if (i != 0)
			Write (", ");

		var type = CSharpLanguage.TypeName (Generator.CppTypeToManaged (parameters [i].Type), Context.Parameter);
		var mangleAs = parameters [i].Type.ToString ();
		if (mangleAs != "" && mangleAs != type)
			Write ("[MangleAs (\"{0}\")] ", mangleAs);
		if (Generator.IsByVal (parameters [i].Type))
			Write ("[ByVal] ");
		if (Generator.IsByVal (parameters [i].Type))
			Write ("[ByVal] ");
		if (Generator.IsCppType (parameters [i].Type))
			Write ("[MarshalAs (UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof ("+type+".Marshaler))] ");

		Write (type);
		Write (" ");
		Write (CSharpLanguage.SafeIdentifier (parameters [i].Name));
	}
}

        #line default
        #line hidden
        
        
        public override string TransformText() {
            this.GenerationEnvironment = null;
            
            #line 6 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"

	var @namespace = Generator.Lib.BaseNamespace + (Function.ParentNamespace != null? "." + string.Join (".", Function.ParentNamespace.FullyQualifiedName) : "");

if (!Nested) {

            
            #line default
            #line hidden
            
            #line 11 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write("\r\n// -------------------------------------------------------------------------\r\n/" +
                    "/  Managed delegate for ");
            
            #line default
            #line hidden
            
            #line 13 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( Function.Name ));
            
            #line default
            #line hidden
            
            #line 13 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write("\r\n//  Generated from ");
            
            #line default
            #line hidden
            
            #line 14 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( Path.GetFileName (Generator.InputFileName) ));
            
            #line default
            #line hidden
            
            #line 14 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(" on ");
            
            #line default
            #line hidden
            
            #line 14 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( DateTime.Now ));
            
            #line default
            #line hidden
            
            #line 14 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write("\r\n//\r\n//  This file was auto generated. Do not edit.\r\n// ------------------------" +
                    "-------------------------------------------------\r\n\r\nusing System;\r\nusing System" +
                    ".Runtime.InteropServices;\r\nusing Mono.Cxxi;\r\n\r\nnamespace ");
            
            #line default
            #line hidden
            
            #line 23 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( @namespace ));
            
            #line default
            #line hidden
            
            #line 23 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(" {\r\n");
            
            #line default
            #line hidden
            
            #line 24 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
 } /* if !Nested */ 
            
            #line default
            #line hidden
            
            #line 25 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"

	var returnType = CSharpLanguage.TypeName (Generator.CppTypeToManaged (Function.ReturnType), Context.Wrapper | Context.Return);

            
            #line default
            #line hidden
            
            #line 28 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write("\t[UnmanagedFunctionPointer (CallingConvention.Cdecl)]\r\n\t");
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( Function.Access.ToString() ));
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(" delegate ");
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( returnType ));
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(" ");
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture( Function.FormattedName ));
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(" (");
            
            #line default
            #line hidden
            
            #line 29 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
 WriteParameters (Function.Parameters); 
            
            #line default
            #line hidden
            
            #line 30 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write(");\r\n\r\n");
            
            #line default
            #line hidden
            
            #line 32 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
 if (!Nested) { 
            
            #line default
            #line hidden
            
            #line 33 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
            this.Write("}\r\n");
            
            #line default
            #line hidden
            
            #line 34 "C:\Users\Kevin\code\VisualStudio\cxxi\src\generator\Templates\CSharp\CSharpFunction.tt"
 } 
            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
        
        protected override void Initialize() {
            base.Initialize();
        }
    }
}
