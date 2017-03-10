

namespace dg.common.validation
{
    public class ValidationError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public object AttemptedValue { get; set; }
        public string PropertyName { get; set; }
        public string ResourceName { get; set; }
        //   public Severity Severity { get; set; }

        public override string ToString()
        {
            return string.Format("ValidaionError [{0},{1},{2}-{3}]", 
                                 ErrorCode, ErrorMessage, PropertyName, AttemptedValue);
        }
    }
}
