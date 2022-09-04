namespace EnTier.Results
{
    public  class Result
    {
        public bool Success { get; set; }

        public Result Succeed()
        {
            Success = true;

            return this;
        }
        
        public Result Fail()
        {
            Success = false;

            return this;
        }
        
        public static bool operator ==(Result value, bool bValue)
        {
            return value?.Success == bValue;
        }

        public static bool operator !=(Result value, bool bValue)
        {
            return !(value == bValue);
        }
        
        public static implicit operator bool(Result r) => r.Success;
    }
}