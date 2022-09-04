namespace EnTier.Results
{
    public class Result<T1, T2>:Result
    {
        public T2 Secondary { get; set; }
        public T1 Primary { get; set; }

        public Result()
        {
            Success = false;
            Primary = default;
            Secondary = default;
        }

        public Result(bool success, T2 secondary, T1 primary)
        {
            Success = success;
            Secondary = secondary;
            Primary = primary;
        }

        public Result<T1, T2> FailAndDefaultBothValues()
        {
            this.Success = false;

            Primary = default;

            Secondary = default;
            
            return this;
        }

        public Result<T1, T2> Succeed(T1 p, T2 s)
        {
            this.Success = true;
            this.Primary = p;
            this.Secondary = s;
            return this;
        }
    }
}