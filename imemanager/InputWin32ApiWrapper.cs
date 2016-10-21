using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ActiveXObject
{
    public class InputWin32ApiWrapper
    {

        /// <summary>英数字入力モード</summary>
        public const int IME_CMODE_ALPHANUMERIC = 0x0;
        /// <summary>言語依存入力モード</summary>
        public const int IME_CMODE_NATIVE = 0x1;
        /// <summary>日本語入力モード</summary>
        public const int IME_CMODE_JAPANESE = IME_CMODE_NATIVE;
        /// <summary>カタカナ入力モード</summary>
        public const int IME_CMODE_KATAKANA = 0x2;
        /// <summary>言語入力モード</summary>
        public const int IME_CMODE_LANGUAGE = 0x3;
        /// <summary>全角入力モード</summary>
        public const int IME_CMODE_FULLSHAPE = 0x8;
        /// <summary>ローマ字入力モード</summary>
        public const int IME_CMODE_ROMAN = 0x10;

        // 変換モードを示す定数の宣言
        /// <summary>無変換モード</summary>
        public const int IME_SMODE_NONE = 0x0;
        /// <summary>複数文字変換モード</summary>
        public const int IME_SMODE_PLAURALCLAUSE = 0x1;
        /// <summary>単一文字変換モード</summary>
        public const int IME_SMODE_SINGLECONVERT = 0x2;
        /// <summary>自動変換モード</summary>
        public const int IME_SMODE_AUTOMATIC = 0x4;
        /// <summary>予測変換モード</summary>
        public const int IME_SMODE_PHRASEPREDICT = 0x8;

        public const int DEFAULT_IME_MODE = 9;

        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("Imm32.dll")]
        public static extern bool ImmGetConversionStatus(IntPtr hIMC, ref int fdwConversion, ref int fdwSentence);

        [DllImport("Imm32.dll")]
        public static extern bool ImmGetOpenStatus(IntPtr hIMC);

        [DllImport("Imm32.dll")]
        public static extern bool ImmSetOpenStatus(IntPtr hIMC, bool fOpen);

        [DllImport("Imm32.dll")]
        public static extern bool ImmSetConversionStatus(IntPtr hIMC, int fdwConversion, int fdwSentence);

        public delegate bool EnumInputContextProc(IntPtr hWnd, IntPtr parameter);

        [DllImport("Imm32.dll")]
        public static extern bool ImmEnumInputContext(int threadId, EnumInputContextProc callback, IntPtr parameter);

        public static int lastConversion = 0;

        public static int GetInputConversion()
        {
            var result = GetAllInputContext(0);
            if (result == null)
            {
                return DEFAULT_IME_MODE;
            }
            foreach (var ctx in result)
            {
                var lngStatusIMEConversion = 0;
                var lngStatusSentance = 0;
                /*if (!ImmGetOpenStatus(ctx))
                {
                    ImmSetOpenStatus(ctx, true);
                }*/
                var lngWin32apiResultCode = ImmGetConversionStatus(ctx, ref lngStatusIMEConversion, ref lngStatusSentance);
                if (lngWin32apiResultCode)
                {
                    return lngStatusIMEConversion;
                }
            }
            return DEFAULT_IME_MODE;
        }

        public static int RestoreInputConversion(int ime)
        {
            var result = GetAllInputContext(0);
            if (result == null)
            {
                return -1;
            }
            foreach (var ctx in result)
            {
                var lngStatusIMEConversion = 0;
                var lngStatusSentance = 0;
                if (!ImmGetOpenStatus(ctx))
                {
                    ImmSetOpenStatus(ctx, true);
                }
                var lngWin32apiResultCode = ImmGetConversionStatus(ctx, ref lngStatusIMEConversion, ref lngStatusSentance);
                if (lngWin32apiResultCode)
                {
                    if (lngStatusIMEConversion == lastConversion)
                    {
                        lngWin32apiResultCode = ImmSetConversionStatus(ctx, ime, GetSentenceByImeMode(ime));
                        lngWin32apiResultCode = ImmGetConversionStatus(ctx, ref lngStatusIMEConversion, ref lngStatusSentance);
                        return lngStatusIMEConversion;
                    }
                }
            }
            return -1;
        }

        public static int SetInputConversion(string imeMode)
        {
            var result = GetAllInputContext(0);
            if (result == null)
            {
                return -1;
            }
            foreach (var ctx in result)
            {
                if (!ImmGetOpenStatus(ctx))
                {
                    ImmSetOpenStatus(ctx, true);
                }
                var lngStatusIMEConversion = GetConversionByImeModeString(imeMode);
                var lngStatusImeSentence = GetSentenceByImeModeString(imeMode);
                var lngWin32apiResultCode = ImmSetConversionStatus(ctx, lngStatusIMEConversion, lngStatusImeSentence);

                lngWin32apiResultCode = ImmGetConversionStatus(ctx, ref lngStatusIMEConversion, ref lngStatusImeSentence);
                lastConversion = lngStatusIMEConversion;
                return lngStatusIMEConversion;
            }
            return 0;
        }

        public static int GetSentenceByImeMode(int imeMode)
        {
            switch(imeMode)
            {
                case 25:
                   return IME_SMODE_PHRASEPREDICT;
                default:
                    return IME_SMODE_NONE;
            }
        }

        public static int GetConversionByImeModeString(string imeMode)
        {
            ImeMode mode;
            if (!Enum.TryParse(imeMode, out mode)) {
                return DEFAULT_IME_MODE;
            }
            switch(mode)
            {
                case ImeMode.Alpha:
                    return 0;
                case ImeMode.AlphaFull: // 全角英数字
                    return IME_CMODE_FULLSHAPE;
                case ImeMode.Hiragana: // 全角ひらがな
                    return IME_CMODE_NATIVE | IME_CMODE_FULLSHAPE;
                case ImeMode.Katakana: // 全角カナ
                    return IME_CMODE_NATIVE | IME_CMODE_FULLSHAPE | IME_CMODE_KATAKANA;
                case ImeMode.KatakanaHalf: // 半角カナ
                    return IME_CMODE_NATIVE | IME_CMODE_KATAKANA;
                default:
                    return DEFAULT_IME_MODE;
            }
        }


        public static int GetSentenceByImeModeString(string imeMode)
        {
            ImeMode mode;
            if (!Enum.TryParse(imeMode, out mode))
            {
                return 0;
            }
            switch (mode)
            {
                case ImeMode.Alpha:
                    return IME_SMODE_NONE;
                case ImeMode.AlphaFull: // 全角英数字
                    return IME_SMODE_NONE;
                case ImeMode.Hiragana: // 全角ひらがな
                    return IME_SMODE_PHRASEPREDICT;
                case ImeMode.Katakana: // 全角カナ
                    return IME_SMODE_NONE;
                case ImeMode.KatakanaHalf: // 半角カナ
                    return IME_SMODE_NONE;
                default:
                    return IME_SMODE_PHRASEPREDICT;
            }
        }

        public static bool EnumInputContextCallback(IntPtr handle, IntPtr pointer)
        {
            // Creates a managed GCHandle object from the pointer representing a handle to the list created in GetChildWindows.
            var gcHandle = GCHandle.FromIntPtr(pointer);

            // Casts the handle back back to a List<IntPtr>
            var list = gcHandle.Target as List<IntPtr>;

            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }

            // Adds the handle to the list.
            list.Add(handle);

            return true;
        }

        // Returns an IEnumerable<IntPtr> containing the handles of all child windows of the parent window.
        public static IEnumerable<IntPtr> GetAllInputContext(int threadId)
        {
            // Create list to store child window handles.
            var result = new List<IntPtr>();

            // Allocate list handle to pass to EnumChildWindows.
            var listHandle = GCHandle.Alloc(result);

            try
            {
                // Enumerates though all the child windows of the parent represented by IntPtr parent, executing EnumChildWindowsCallback for each. 
                ImmEnumInputContext(threadId, EnumInputContextCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                // Free the list handle.
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }

            // Return the list of child window handles.
            return result;
        }
    }
}
