using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;

namespace DepthCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string source_path = "C:\\Users\\Joe bingle\\Downloads\\DepthCalc\\HInfinite_256.png";
        public static Bitmap source_image;
        public MainWindow()
        {
            algos.main = this;
            InitializeComponent();



            source_image = get_source();

            //result.visualized_img = 
            //result.visualized_img

            //visuals_panel;

            VisualizerControl vc1 = new(this, algos.get_algo(0));
            visuals_panel.Children.Add(vc1);

            //VisualizerControl vc2 = new(this, algos.get_algo(1));
            //visuals_panel.Children.Add(vc2);

            VisualizerControl vc3 = new(this, algos.get_algo(2));
            visuals_panel.Children.Add(vc3);

            VisualizerControl vc4 = new(this, algos.get_algo(3));
            visuals_panel.Children.Add(vc4);
        }


        public Bitmap get_source(){
            return new Bitmap(source_path);
        }

        public static BitmapImage BitmapToImageSource(Bitmap bitmap){
            using (MemoryStream memory = new MemoryStream()){
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
        }}
    }


    // ///////////////////////////// //
    // BITMAP VISUALIZER ALGORITHMS //
    // /////////////////////////// //
    public static class algos{
        public static MainWindow main;
        // algo params 
        private static int neighbour_range = 1;

        public static BitmapAlgo get_algo(int index){
            switch (index){
                // regular
                case 0: return new BitmapAlgo { run = algo_unmodified };
                // averaged
                case 1: return new BitmapAlgo { run = algo_average, has_range = true, range = 1 };
                // global brightness
                case 2: return new BitmapAlgo { run = algo_global_brightness };
                // brightness
                case 3: return new BitmapAlgo { run = algo_brightness, has_range = true, range = 1 };
                // BAD INDEX
                default: throw new Exception("Out of bounds algo type index");
        }}

        public class BitmapAlgo{
            public bool has_range = false;
            public int range = 1;

            public bool has_weight = false;
            public double weight = 1.0;

            public Func<Bitmap> run;
            public async void process(System.Windows.Controls.Image item_to_populate){
                // slot in any variables that need setting
                neighbour_range = range;
                item_to_populate.Source = MainWindow.BitmapToImageSource(run());
            }
        }

        static Bitmap algo_unmodified(){
            return main.get_source();
        }
        static Bitmap algo_average(){
            var bitm = main.get_source();
            // iterate all pixels
            for (int x = 0; x < bitm.Width; x++){
                for (int y = 0; y < bitm.Height; y++){

                    int pixels_counted = 0;
                    double pixel_red = 0;
                    double pixel_green = 0;
                    double pixel_blue = 0;
                    // iterate all neighbours
                    for (int offset_x = -neighbour_range; offset_x <= neighbour_range; offset_x++){
                        int current_x = x + offset_x;
                        if (current_x < 0 || current_x >= bitm.Width) continue;

                        for (int offset_y = -neighbour_range; offset_y <= neighbour_range; offset_y++){
                            int current_y = y + offset_y;
                            if (current_y < 0 || current_y >= bitm.Height) continue;
                            if (current_y == y && current_x == x) continue;

                            pixels_counted++;

                            var current_pixel = MainWindow.source_image.GetPixel(current_x, current_y);

                            pixel_red += current_pixel.R;
                            pixel_green += current_pixel.G;
                            pixel_blue += current_pixel.B;
                    }}

                    // then set the average
                    pixel_red /= pixels_counted;
                    pixel_green /= pixels_counted;
                    pixel_blue /= pixels_counted;
                    bitm.SetPixel(x, y, Color.FromArgb((int)pixel_red, (int)pixel_green, (int)pixel_blue)) ;
            }}
            return bitm;
        }
        static Bitmap algo_global_brightness(){
            var bitm = main.get_source();
            // iterate all pixels
            for (int x = 0; x < bitm.Width; x++){
                for (int y = 0; y < bitm.Height; y++){
                    var current_pixel = bitm.GetPixel(x, y);
                    bitm.SetPixel(x, y, pixel_magnitude(current_pixel.R + current_pixel.G + current_pixel.B, 765)) ;
            }}
            return bitm;
        }
        static Bitmap algo_brightness(){
            var bitm = main.get_source();
            // iterate all pixels
            for (int x = 0; x < bitm.Width; x++){
                for (int y = 0; y < bitm.Height; y++){

                    int pixels_counted = 0;
                    double pixel_brightness = 0;
                    // iterate all neighbours
                    for (int offset_x = -neighbour_range; offset_x <= neighbour_range; offset_x++){
                        int current_x = x + offset_x;
                        if (current_x < 0 || current_x >= bitm.Width) continue;

                        for (int offset_y = -neighbour_range; offset_y <= neighbour_range; offset_y++){
                            int current_y = y + offset_y;
                            if (current_y < 0 || current_y >= bitm.Height) continue;
                            if (current_y == y && current_x == x) continue;

                            pixels_counted++;

                            var current_pixel = MainWindow.source_image.GetPixel(current_x, current_y);
                            pixel_brightness += current_pixel.R + current_pixel.G + current_pixel.B;
                    }}

                    pixel_brightness /= pixels_counted;

                    var actual_pixel = bitm.GetPixel(x, y);


                    double actual_brightness = actual_pixel.R + actual_pixel.G + actual_pixel.B;
                    double difference = Math.Abs(actual_brightness - pixel_brightness);
                    difference /= 128;
                    if (difference > 1.0)
                        difference = 1.0;
                    bitm.SetPixel(x, y, pixel_magnitude(difference, 1.0));

                    //actual_brightness /= 765;
                    //actual_brightness -= pixel_brightness / 765;
                    //actual_brightness += 1.0;
                    //actual_brightness /= 2.0;
                    //bitm.SetPixel(x, y, pixel_magnitude(actual_brightness, 1.0));
            }}
            return bitm;
        }








        static Color pixel_magnitude(double value, double max){
            if (value > max) throw new Exception("value cannot be greater than max");
            double multiplier = 255 / max;
            byte multiplied_value = (byte)(value * multiplier);
            return Color.FromArgb(multiplied_value, multiplied_value, multiplied_value);
        }

    }
}
