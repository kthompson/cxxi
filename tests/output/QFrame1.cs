// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Wrappers {
    using System;
    using System.Runtime.InteropServices;
    using Mono.VisualC.Interop;
    
    public class QFrame : QWidget {
        private static IQFrame impl = Wrappers.Libs.Lib.GetClass <IQFrame, _QFrame, QFrame>("QFrame");
        public QFrame(QWidget parent, QFlags<Qt::WindowType> f) {
            impl.QFrame(this.Native, parent, f);
        }
        public int FrameStyle {
            get {
                return impl.frameStyle(this.Native);
            }
            set {
                impl.setFrameStyle(this.Native, value);
            }
        }
        public int FrameWidth {
            get {
                return impl.frameWidth(this.Native);
            }
        }
        public Shape FrameShape {
            get {
                return impl.frameShape(this.Native);
            }
            set {
                impl.setFrameShape(this.Native, value);
            }
        }
        public Shadow FrameShadow {
            get {
                return impl.frameShadow(this.Native);
            }
            set {
                impl.setFrameShadow(this.Native, value);
            }
        }
        public int LineWidth {
            get {
                return impl.lineWidth(this.Native);
            }
            set {
                impl.setLineWidth(this.Native, value);
            }
        }
        public int MidLineWidth {
            get {
                return impl.midLineWidth(this.Native);
            }
            set {
                impl.setMidLineWidth(this.Native, value);
            }
        }
        public QRect FrameRect {
            get {
                return impl.frameRect(this.Native);
            }
            set {
                impl.setFrameRect(this.Native, value);
            }
        }
        public override void Dispose() {
        }
        public interface IQFrame : ICppClassOverridable<QFrame> {
            [Constructor()]
            void QFrame(CppInstancePtr @this, [MangleAs("class QWidget *")] QWidget parent, [MangleAs("class QFlags <Qt::WindowType>")] QFlags<Qt::WindowType> f);
            [Virtual()]
            [Destructor()]
            void Destruct(CppInstancePtr @this);
            [Const()]
            int frameStyle(CppInstancePtr @this);
            void setFrameStyle(CppInstancePtr @this, [MangleAs("int")] int value);
            [Const()]
            int frameWidth(CppInstancePtr @this);
            [Const()]
            Shape frameShape(CppInstancePtr @this);
            void setFrameShape(CppInstancePtr @this, [MangleAs("enum Shape")] Shape value);
            [Const()]
            Shadow frameShadow(CppInstancePtr @this);
            void setFrameShadow(CppInstancePtr @this, [MangleAs("enum Shadow")] Shadow value);
            [Const()]
            int lineWidth(CppInstancePtr @this);
            void setLineWidth(CppInstancePtr @this, [MangleAs("int")] int value);
            [Const()]
            int midLineWidth(CppInstancePtr @this);
            void setMidLineWidth(CppInstancePtr @this, [MangleAs("int")] int value);
            [Const()]
            QRect frameRect(CppInstancePtr @this);
            void setFrameRect(CppInstancePtr @this, [MangleAs("class QRect const &")] QRect value);
        }
        private struct _QFrame {
        }
    }
}