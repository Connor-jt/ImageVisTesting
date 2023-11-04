using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static DepthCalc.algos;

namespace DepthCalc{
    /// <summary>
    /// Interaction logic for VisualizerControl.xaml
    /// </summary>
    public partial class VisualizerControl : UserControl{
        readonly MainWindow main;
        readonly BitmapAlgo algorithm;
        public VisualizerControl(MainWindow _main, BitmapAlgo _algorithm){
            main = _main;
            algorithm = _algorithm;
            InitializeComponent();
            reload_image();
            if (algorithm.has_range == false)
                range_panel.Visibility = Visibility.Collapsed;
            if (algorithm.has_weight == false)
                weight_panel.Visibility = Visibility.Collapsed;
        }

        public void reload_image(){
            algorithm.process(Display);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e){
            if (algorithm.has_range == false) return;

            // get value from textbox
            int new_range;
            try{new_range = Convert.ToInt32(rangebox.Text);}catch{return;}
            if (new_range < 0) return;

            // apply new range parameter & reload
            algorithm.range = new_range;
            reload_image();
        }
    }
}
