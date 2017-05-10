using Coc.Data.Interfaces;

namespace Coc.Data.Xmi.AbstractValidator
{
    /*
    /// <summary>
    /// <img src="images/Xmi.AbstractValidator.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class ValidatorFactory
    {
        public static Validator CreateValidator()
        {
#if PL_FUNCTIONAL_TESTING
            return new FunctionalValidator.FunctionalValidator();
#elif PL_PERFORMANCE_TESTING
            return new PerformanceValidator.PerformanceValidator();
#else
            return new Validator();
#endif
        }
    }
}