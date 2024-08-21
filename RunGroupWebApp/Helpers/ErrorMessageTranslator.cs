namespace RunGroupWebApp.Helpers
{
    public static class ErrorMessageTranslator
    {
        public static string GetPasswordErrorMessages(string message)
        {
            if (message == "PasswordTooShort")
            {
                return "密碼長度至少要6個字元";
            }
            else if (message == "PasswordRequiresLower")
            {
                return "密碼至少要1個小寫英文字母";
            }
            else if (message == "PasswordRequiresNonAlphanumeric")
            {
                return "密碼至少要1個符號";
            }
            else if (message == "PasswordRequiresUpper")
            {
                return "密碼至少要1個大寫英文字母";
            }
            else if (message == "PasswordRequiresDigit")
            {
                return "密碼至少要1個數字";
            }
            return "密碼格式須滿足條件";
        }
    }
}
