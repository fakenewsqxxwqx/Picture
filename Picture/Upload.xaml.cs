using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = System.IO.Path;

namespace Picture
{
    /// <summary>
    /// Upload.xaml 的交互逻辑
    /// </summary>
    public partial class Upload : Window, INotifyPropertyChanged
    {
        private BitmapImage selectedImage;
        public BitmapImage SelectedImage
        {
            get { return selectedImage; }
            set
            {
                selectedImage = value;
                OnPropertyChanged(nameof(SelectedImage));
            }
        }

        private ObservableCollection<string> categories;
        public ObservableCollection<string> Categories
        {
            get { return categories; }
            set
            {
                categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        private string selectedCategory;
        public string SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
            }
        }

        private string selectedImageName;
        public string SelectedImageName
        {
            get { return selectedImageName; }
            set
            {
                selectedImageName = value;
                OnPropertyChanged(nameof(SelectedImageName));
            }
        }

        private string selectedImageExt;
        public string SelectedImageExt
        {
            get { return selectedImageExt; }
            set
            {
                selectedImageExt = value;
                OnPropertyChanged(nameof(SelectedImageExt));
            }
        }

        public Upload()
        {
            InitializeComponent();
            DataContext = this;

            // 初始化分类下拉框的数据
            Categories = new ObservableCollection<string>
            {
                "cat",
                "dog",
                "wild"
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btn_SelectPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.gif)|*.png;*.jpg;*.jpeg;*.gif";
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImage = new BitmapImage(new Uri(openFileDialog.FileName));
                SelectedImageName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                SelectedImageExt = System.IO.Path.GetExtension(openFileDialog.FileName);
            }
        }

        private void btn_Submit_Click(object sender, RoutedEventArgs e)
        {
            // 检查是否选择了图片
            if (SelectedImage == null)
            {
                MessageBox.Show("请先选择图片");
                return;
            }

            // 检查是否选择了分类
            /*
            if (string.IsNullOrEmpty(SelectedCategory))
            {
                MessageBox.Show("请先选择分类");
                return;
            }
            */
            // 获取项目文件夹路径
            string projectFolderPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

            // 创建保存图片的文件夹（如果不存在）
            string imageFolderPath = Path.Combine(projectFolderPath, "Images");
            if (!Directory.Exists(imageFolderPath))
            {
                Directory.CreateDirectory(imageFolderPath);
            }

            // 生成保存图片的文件路径
            string fileName = SelectedImageName + DateTimeOffset.Now.ToUnixTimeSeconds() + SelectedImageExt;
            string imagePath = Path.Combine(imageFolderPath, fileName);

            // 保存图片
            using (FileStream fileStream = new FileStream(imagePath, FileMode.Create))
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(SelectedImage));
                encoder.Save(fileStream);
            }

            imagePath = imagePath.Replace("\\", "/");

            //如果没有选择分类，由ML模型决定
            if(string.IsNullOrEmpty(SelectedCategory))
            {
                //Load sample data
                var imageBytes = File.ReadAllBytes(imagePath);
                MLModel1.ModelInput sampleData = new MLModel1.ModelInput()
                {
                    ImageSource = imageBytes,
                };

                //Load model and predict output
                string result = MLModel1.Predict(sampleData).PredictedLabel;
                SelectedCategory = result;

            }

            string sql = "insert into picture(title, type, url) values ('" + fileName + "','" + SelectedCategory + "','" + imagePath + "')";

            if (SqlHelper.commonExecute(sql) > 0)
            {
                MessageBox.Show("图片保存成功！");
            }
            else
            {
                MessageBox.Show("图片保存失败！");
            }
        }

        private void cb_Category_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedItem = cb_Category.SelectedItem;

            // Check if an item is selected
            if (selectedItem != null)
            {
                // Get the selected value
                SelectedCategory = cb_Category.SelectedValue.ToString();
            }
        }
    }
}
