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
    
    public class QPixmap : QPaintDevice {
        private static IQPixmap impl = Wrappers.Libs.Lib.GetClass <IQPixmap, _QPixmap, QPixmap>("QPixmap");
        public QPixmap() {
            impl.QPixmap(this.Native);
        }
        public QPixmap(QPixmapData data) {
            impl.QPixmap(this.Native, data);
        }
        public QPixmap(int w, int h) {
            impl.QPixmap(this.Native, w, h);
        }
        public QPixmap(QSize arg0) {
            impl.QPixmap(this.Native, arg0);
        }
        public QPixmap(QString fileName, string format, QFlags<Qt::ImageConversionFlag> flags) {
            impl.QPixmap(this.Native, fileName, format, flags);
        }
        public QPixmap(string[] xpm) {
            impl.QPixmap(this.Native, xpm);
        }
        public QPixmap(QPixmap arg0) {
            impl.QPixmap(this.Native, arg0);
        }
        public bool IsNull {
            get {
                return impl.isNull(this.Native);
            }
        }
        public int Width {
            get {
                return impl.width(this.Native);
            }
        }
        public int Height {
            get {
                return impl.height(this.Native);
            }
        }
        public QSize Size {
            get {
                return impl.size(this.Native);
            }
        }
        public QRect Rect {
            get {
                return impl.rect(this.Native);
            }
        }
        public int Depth {
            get {
                return impl.depth(this.Native);
            }
        }
        public QBitmap Mask {
            get {
                return impl.mask(this.Native);
            }
            set {
                impl.setMask(this.Native, value);
            }
        }
        public QPixmap AlphaChannel {
            get {
                return impl.alphaChannel(this.Native);
            }
            set {
                impl.setAlphaChannel(this.Native, value);
            }
        }
        public bool HasAlpha {
            get {
                return impl.hasAlpha(this.Native);
            }
        }
        public bool HasAlphaChannel {
            get {
                return impl.hasAlphaChannel(this.Native);
            }
        }
        public QImage ToImage {
            get {
                return impl.toImage(this.Native);
            }
        }
        public int SerialNumber {
            get {
                return impl.serialNumber(this.Native);
            }
        }
        public long CacheKey {
            get {
                return impl.cacheKey(this.Native);
            }
        }
        public bool IsDetached {
            get {
                return impl.isDetached(this.Native);
            }
        }
        public bool IsQBitmap {
            get {
                return impl.isQBitmap(this.Native);
            }
        }
        public QX11Info X11Info {
            get {
                return impl.x11Info(this.Native);
            }
        }
        public ulong X11PictureHandle {
            get {
                return impl.x11PictureHandle(this.Native);
            }
        }
        public ulong Handle {
            get {
                return impl.handle(this.Native);
            }
        }
        public QPixmapData PixmapData {
            get {
                return impl.pixmapData(this.Native);
            }
        }
        public override void Dispose() {
        }
        public static int DefaultDepth() {
            return impl.defaultDepth();
        }
        public void Fill(QColor fillColor) {
            impl.fill(this.Native, fillColor);
        }
        public void Fill(QWidget widget, QPoint ofs) {
            impl.fill(this.Native, widget, ofs);
        }
        public QBitmap CreateHeuristicMask(bool clipTight) {
            return impl.createHeuristicMask(this.Native, clipTight);
        }
        public QBitmap CreateMaskFromColor(QColor maskColor) {
            return impl.createMaskFromColor(this.Native, maskColor);
        }
        public QBitmap CreateMaskFromColor(QColor maskColor, MaskMode mode) {
            return impl.createMaskFromColor(this.Native, maskColor, mode);
        }
        public static QPixmap GrabWindow(ulong arg0, int x, int y, int w, int h) {
            return impl.grabWindow(arg0, x, y, w, h);
        }
        public static QPixmap GrabWidget(QWidget widget, QRect rect) {
            return impl.grabWidget(widget, rect);
        }
        public QPixmap Scaled(QSize s, AspectRatioMode aspectMode, TransformationMode mode) {
            return impl.scaled(this.Native, s, aspectMode, mode);
        }
        public QPixmap ScaledToWidth(int w, TransformationMode mode) {
            return impl.scaledToWidth(this.Native, w, mode);
        }
        public QPixmap ScaledToHeight(int h, TransformationMode mode) {
            return impl.scaledToHeight(this.Native, h, mode);
        }
        public QPixmap Transformed(QMatrix arg0, TransformationMode mode) {
            return impl.transformed(this.Native, arg0, mode);
        }
        public static QMatrix TrueMatrix(QMatrix m, int w, int h) {
            return impl.trueMatrix(m, w, h);
        }
        public QPixmap Transformed(QTransform arg0, TransformationMode mode) {
            return impl.transformed(this.Native, arg0, mode);
        }
        public static QTransform TrueMatrix(QTransform m, int w, int h) {
            return impl.trueMatrix(m, w, h);
        }
        public static QPixmap FromImage(QImage image, QFlags<Qt::ImageConversionFlag> flags) {
            return impl.fromImage(image, flags);
        }
        public bool Load(QString fileName, string format, QFlags<Qt::ImageConversionFlag> flags) {
            return impl.load(this.Native, fileName, format, flags);
        }
        public bool LoadFromData(string buf, uint len, string format, QFlags<Qt::ImageConversionFlag> flags) {
            return impl.loadFromData(this.Native, buf, len, format, flags);
        }
        public bool Save(QString fileName, string format, int quality) {
            return impl.save(this.Native, fileName, format, quality);
        }
        public bool Save(QIODevice device, string format, int quality) {
            return impl.save(this.Native, device, format, quality);
        }
        public QPixmap Copy(QRect rect) {
            return impl.copy(this.Native, rect);
        }
        public void Scroll(int dx, int dy, QRect rect, QRegion exposed) {
            impl.scroll(this.Native, dx, dy, rect, exposed);
        }
        public void Detach() {
            impl.detach(this.Native);
        }
        public static QPixmap FromX11Pixmap(ulong pixmap, ShareMode mode) {
            return impl.fromX11Pixmap(pixmap, mode);
        }
        public static int X11SetDefaultScreen(int screen) {
            return impl.x11SetDefaultScreen(screen);
        }
        public void X11SetScreen(int screen) {
            impl.x11SetScreen(this.Native, screen);
        }
        public interface IQPixmap : ICppClassOverridable<QPixmap> {
            [Constructor()]
            void QPixmap(CppInstancePtr @this);
            [Constructor()]
            void QPixmap(CppInstancePtr @this, [MangleAs("struct QPixmapData *")] QPixmapData data);
            [Constructor()]
            void QPixmap(CppInstancePtr @this, [MangleAs("int")] int w, [MangleAs("int")] int h);
            [Constructor()]
            void QPixmap(CppInstancePtr @this, [MangleAs("class QSize const &")] QSize arg0);
            [Constructor()]
            void QPixmap(CppInstancePtr @this, [MangleAs("class QString const &")] QString fileName, [MangleAs("char const *")] string format, [MangleAs("class QFlags <Qt::ImageConversionFlag>")] QFlags<Qt::ImageConversionFlag> flags);
            [Constructor()]
            void QPixmap(CppInstancePtr @this, [MangleAs("char const * const *")] string[] xpm);
            [Constructor()]
            void QPixmap(CppInstancePtr @this, [MangleAs("class QPixmap const &")] QPixmap arg0);
            [Virtual()]
            [Destructor()]
            void Destruct(CppInstancePtr @this);
            [Const()]
            bool isNull(CppInstancePtr @this);
            [Const()]
            int width(CppInstancePtr @this);
            [Const()]
            int height(CppInstancePtr @this);
            [Const()]
            QSize size(CppInstancePtr @this);
            [Const()]
            QRect rect(CppInstancePtr @this);
            [Const()]
            int depth(CppInstancePtr @this);
            [Static()]
            int defaultDepth();
            void fill(CppInstancePtr @this, [MangleAs("class QColor const &")] QColor fillColor);
            void fill(CppInstancePtr @this, [MangleAs("class QWidget const *")] QWidget widget, [MangleAs("class QPoint const &")] QPoint ofs);
            [Const()]
            QBitmap mask(CppInstancePtr @this);
            void setMask(CppInstancePtr @this, [MangleAs("class QBitmap const &")] QBitmap value);
            [Const()]
            QPixmap alphaChannel(CppInstancePtr @this);
            void setAlphaChannel(CppInstancePtr @this, [MangleAs("class QPixmap const &")] QPixmap value);
            [Const()]
            bool hasAlpha(CppInstancePtr @this);
            [Const()]
            bool hasAlphaChannel(CppInstancePtr @this);
            [Const()]
            QBitmap createHeuristicMask(CppInstancePtr @this, [MangleAs("bool")] bool clipTight);
            [Const()]
            QBitmap createMaskFromColor(CppInstancePtr @this, [MangleAs("class QColor const &")] QColor maskColor);
            [Const()]
            QBitmap createMaskFromColor(CppInstancePtr @this, [MangleAs("class QColor const &")] QColor maskColor, [MangleAs("enum MaskMode")] MaskMode mode);
            [Static()]
            QPixmap grabWindow([MangleAs("int long unsigned")] ulong arg0, [MangleAs("int")] int x, [MangleAs("int")] int y, [MangleAs("int")] int w, [MangleAs("int")] int h);
            [Static()]
            QPixmap grabWidget([MangleAs("class QWidget *")] QWidget widget, [MangleAs("class QRect const &")] QRect rect);
            [Const()]
            QPixmap scaled(CppInstancePtr @this, [MangleAs("class QSize const &")] QSize s, [MangleAs("enum AspectRatioMode")] AspectRatioMode aspectMode, [MangleAs("enum TransformationMode")] TransformationMode mode);
            [Const()]
            QPixmap scaledToWidth(CppInstancePtr @this, [MangleAs("int")] int w, [MangleAs("enum TransformationMode")] TransformationMode mode);
            [Const()]
            QPixmap scaledToHeight(CppInstancePtr @this, [MangleAs("int")] int h, [MangleAs("enum TransformationMode")] TransformationMode mode);
            [Const()]
            QPixmap transformed(CppInstancePtr @this, [MangleAs("class QMatrix const &")] QMatrix arg0, [MangleAs("enum TransformationMode")] TransformationMode mode);
            [Static()]
            QMatrix trueMatrix([MangleAs("class QMatrix const &")] QMatrix m, [MangleAs("int")] int w, [MangleAs("int")] int h);
            [Const()]
            QPixmap transformed(CppInstancePtr @this, [MangleAs("class QTransform const &")] QTransform arg0, [MangleAs("enum TransformationMode")] TransformationMode mode);
            [Static()]
            QTransform trueMatrix([MangleAs("class QTransform const &")] QTransform m, [MangleAs("int")] int w, [MangleAs("int")] int h);
            [Const()]
            QImage toImage(CppInstancePtr @this);
            [Static()]
            QPixmap fromImage([MangleAs("class QImage const &")] QImage image, [MangleAs("class QFlags <Qt::ImageConversionFlag>")] QFlags<Qt::ImageConversionFlag> flags);
            bool load(CppInstancePtr @this, [MangleAs("class QString const &")] QString fileName, [MangleAs("char const *")] string format, [MangleAs("class QFlags <Qt::ImageConversionFlag>")] QFlags<Qt::ImageConversionFlag> flags);
            bool loadFromData(CppInstancePtr @this, [MangleAs("char unsigned const *")] string buf, [MangleAs("int unsigned")] uint len, [MangleAs("char const *")] string format, [MangleAs("class QFlags <Qt::ImageConversionFlag>")] QFlags<Qt::ImageConversionFlag> flags);
            [Const()]
            bool save(CppInstancePtr @this, [MangleAs("class QString const &")] QString fileName, [MangleAs("char const *")] string format, [MangleAs("int")] int quality);
            [Const()]
            bool save(CppInstancePtr @this, [MangleAs("class QIODevice *")] QIODevice device, [MangleAs("char const *")] string format, [MangleAs("int")] int quality);
            [Const()]
            QPixmap copy(CppInstancePtr @this, [MangleAs("class QRect const &")] QRect rect);
            void scroll(CppInstancePtr @this, [MangleAs("int")] int dx, [MangleAs("int")] int dy, [MangleAs("class QRect const &")] QRect rect, [MangleAs("class QRegion *")] QRegion exposed);
            [Const()]
            int serialNumber(CppInstancePtr @this);
            [Const()]
            long cacheKey(CppInstancePtr @this);
            [Const()]
            bool isDetached(CppInstancePtr @this);
            void detach(CppInstancePtr @this);
            [Const()]
            bool isQBitmap(CppInstancePtr @this);
            [Static()]
            QPixmap fromX11Pixmap([MangleAs("int long unsigned")] ulong pixmap, [MangleAs("enum ShareMode")] ShareMode mode);
            [Static()]
            int x11SetDefaultScreen([MangleAs("int")] int screen);
            void x11SetScreen(CppInstancePtr @this, [MangleAs("int")] int screen);
            [Const()]
            QX11Info x11Info(CppInstancePtr @this);
            [Const()]
            ulong x11PictureHandle(CppInstancePtr @this);
            [Const()]
            ulong handle(CppInstancePtr @this);
            [Const()]
            QPixmapData pixmapData(CppInstancePtr @this);
        }
        private struct _QPixmap {
            // FIXME: Unknown type "class QExplicitlySharedDataPointer <QPixmapData>" for field "data." Assuming IntPtr.
            private System.IntPtr data;
        }
    }
}