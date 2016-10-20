using System;
using System.Runtime.InteropServices;


namespace ActiveXObject
{

    [ProgId("IMEManager")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("7A5D58C7-1C27-4DFF-8C8F-F5876FF94C64")]
    public class IMEManager 
    {
        [ComVisible(true)]
        public int GetCurrentInputConversion()
        {
            return InputWin32ApiWrapper.GetInputConversion();
        }

        [ComVisible(true)]
        public int RestoreInputConversion(int imeMode) {
            return InputWin32ApiWrapper.RestoreInputConversion(imeMode);
        }

        [ComVisible(true)] 
        public int SetInputConversion(string imeMode)
        {
            return InputWin32ApiWrapper.SetInputConversion(imeMode);
        }

    }
}