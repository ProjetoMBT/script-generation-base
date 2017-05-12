using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PlugSpl.DataStructs.ProductConfigurator;

namespace ComponentPoolManager {
    public interface IPoolManagerCodeHandler {
        string GetName();
        void GenerateCode(DanuProductConfigurator danu);
    }
}
