namespace AuthTest.ViewModels
{
    public class OTPVM
    {
        public char[] OTP { set; get; } = new char[6];
        public bool valid { set; get; }

    }
}
