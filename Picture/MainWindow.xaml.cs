using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace Picture
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class ImageListViewModel
        {
            public ObservableCollection<ImageItem> ImageList { get; set; }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ImageListViewModel();
        }

        private void btn_Cat_Click(object sender, RoutedEventArgs e)
        {
            GetData("cat");
        }

        private void btn_Dog_Click(object sender, RoutedEventArgs e)
        {
            GetData("dog");
        }

        private void btn_Wild_Click(object sender, RoutedEventArgs e)
        {
            GetData("wild");
        }

        private void GetData(string type)
        {
            ImageListViewModel imageListViewModel = new ImageListViewModel();
            imageListViewModel.ImageList = new ObservableCollection<ImageItem>();
            string sql = "select Id,title,type,url from picture where type = '" + type + "'";

            DataTable dt = SqlHelper.query(sql);

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ImageItem imageItem = new ImageItem()
                    {
                        ImageName = TruncateImageName(dr["title"].ToString()),
                        ImagePath = dr["url"].ToString()
                    };

                    imageListViewModel.ImageList.Add(imageItem);
                }
            }
            DataContext = imageListViewModel;
        }

        private void btn_Add_Click(object sender, RoutedEventArgs e)
        {
            Upload upload = new Upload();
            upload.ShowDialog();
        }

        public string TruncateImageName(string imageName)
        {
            const int maxLength = 15;

            if (imageName.Length <= maxLength)
            {
                return imageName;
            }
            else
            {
                string extension = Path.GetExtension(imageName);
                string nameWithoutExtension = Path.GetFileNameWithoutExtension(imageName);
                string truncatedName = nameWithoutExtension.Substring(0, maxLength - extension.Length);
                return truncatedName + extension;
            }
        }
    }

    public class ImageItem : INotifyPropertyChanged
    {
        private string _imagePath;
        private string _imageName;

        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public string ImageName
        {
            get { return _imageName; }
            set
            {
                _imageName = value;
                OnPropertyChanged(nameof(ImageName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
