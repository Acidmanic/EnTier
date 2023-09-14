namespace Example.Prepopulation;

public class AdminUserNameProvider:IUserNameProvider
{
    public string Username() => "Administrator";


    public string FullName() => "Acidmanic Moayedi";
}