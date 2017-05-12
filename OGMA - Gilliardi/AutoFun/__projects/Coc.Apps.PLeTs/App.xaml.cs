using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;

namespace Coc.Apps.PLeTs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {

#if PL_OATS 
            runOATS();
#elif PL_MTM 
            runDefault();
#elif PL_LR
            runDefault();

#endif

        }


        public void runOATS()
        {
            MainWindowOATS form = new MainWindowOATS();
            form.Show();
        }

        private void runDefault(){
            MainWindow form = new MainWindow();
            form.Show();
        }


    }
}
