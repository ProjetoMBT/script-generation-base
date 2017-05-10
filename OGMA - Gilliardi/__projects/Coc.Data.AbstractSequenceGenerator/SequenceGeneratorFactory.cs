//#define HSI
//#define DFS
//#define Wp

using Coc.Data.Interfaces;
using Coc.Data.ControlStructure;
#if HSI
using Coc.Data.HSI;
#endif
#if DFS
using Coc.Data.DFS;
#endif
#if WP
using Coc.Data.Wpartial;
#endif

namespace Coc.Data.AbstractSequenceGenerator
{
    /*
    /// <summary>
    /// <img src="images/AbstractSequenceGenerator.PNG"/>
    /// </summary>
    public static class NamespaceDoc
    {
    }*/



    public class SequenceGeneratorFactory
    {
        public static SequenceGenerator CreateSequenceGenerator(StructureType type)
        {
            switch (type)
            {
#if HSI
                case StructureType.HSI:
                    return new HsiMethod();
#endif
#if DFS
                case StructureType.DFS:
                    return new DepthFirstSearch();
#endif
#if WP
                case StructureType.Wp:
                    return new Wp();
#endif
#if OATS
                case StructureType.OATS:
                    return new SequenceGenerator();
#endif

            }
            return null;
        }
    }
}
