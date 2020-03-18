public class DefaultLoginService : ILoginService
{
    public LoginResponse Login(string username, string passsword)
    {
        if ("mani"==username.ToLower()&& "admin"==passsword){
            return new LoginResponse{
                Success = true,
                Token = "ab98jf8fmf0bijdlfkmdfbdfi34jregfuij4k4t09jdlskgdsg3t4"
            };
        }

        return new LoginResponse {

            Message = "Invalid credentials",
            Success = false
        };
    }
}