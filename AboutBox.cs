using System;
using System.Reflection;
using System.Windows.Forms;

namespace TestTask
{
    partial class AboutBox : Form
    {
        public AboutBox()
        {
            InitializeComponent();
            this.Text = String.Format("О программе {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Версия {0}", AssemblyVersion);
            this.textBoxDescription.Text = "Сортировка: " + System.Environment.NewLine
+ "Для сортировки данных нажмите на название столбца, по которому желаете отсортировать таблицу.Для сортировки по убыванию нажмите на столбец второй раз" + System.Environment.NewLine + System.Environment.NewLine
+ "Фильтрация:" + System.Environment.NewLine 
+ "Для фильтрации данных выберите желаемые значения полей 'Статус', 'Отдел', 'Должность' и введите фамилию или часть фамилии в поле ввода фамилии. После этого нажмите на кнопку 'Отфильтровать'." + System.Environment.NewLine
+ "Кнопка 'Очистить фильтры' очищает поля для ввода фильтров и выводит всю таблицу." + System.Environment.NewLine + System.Environment.NewLine
+ "Статистика:" + System.Environment.NewLine
+ "Для вывода статистики введите начальную и конечную дату, выберите желаемое значение полей 'Статус' и 'Устроен/Уволен'.После этого в таблицу будут выведены записи подходящие по заданным параметрам, отсортированные по дате приема на работу / увольнения. текстовое поле будет выведенно количество принятых / уволенных работников." + System.Environment.NewLine + System.Environment.NewLine;
        }

        #region Методы доступа к атрибутам сборки

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
        #endregion
    }
}
