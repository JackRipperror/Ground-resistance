using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Ground_resistance
{
    public partial class Form1 : Form
    {
        // === ГЛОБАЛЬНЫЕ ПЕРЕМЕННЫЕ ===
        public double Yc1 = 0;
        public double Yc2 = 0;
        public double K = 0;
        public double h = 0;
        public double u = 0;
        public double SC = 0;
        public double yn = 0;
        public double yv = 0;
        public double a = 0;
        public double b = 0;
        public double c = 0;
        public double R = 0;
        public double Apl = 0;
        public double Psr = 0;
        public double Pkr = 0;
        public string result = string.Empty;

        private Dictionary<int, (double a, double b, double c)> myTable1;
        private Dictionary<int, List<RangeCoeff>> coeffTableYc2;


        // === КЛАСС ДЛЯ ХРАНЕНИЯ ДИАПАЗОНА ===
        public class RangeCoeff
        {
            public double Min { get; set; }
            public double Max { get; set; }
            public double Coeff { get; set; }

            public RangeCoeff(double min, double max, double coeff)
            {
                Min = min;
                Max = max;
                Coeff = coeff;
            }
        }

        // === КОНСТРУКТОР ===
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            InitCoeffTable();
            InitComboBoxes();
            InitTable();
        }

        // === ЗАПОЛНЕНИЕ КОМБОБОКСОВ ===
        private void InitComboBoxes()
        {
            string[] TipeGroundData =
            {
                "Крупнообломочные, пески кр. ср.",
                "Пески мелкие",
                "Пески пылеватые",
                "Глинистые IL <= 0,25",
                "Глинистые 0,25 < IL <= 0,5",
                "Глинистые IL > 0,5"
            };
            TipeGround.Items.AddRange(TipeGroundData);
            TipeGround.SelectedIndex = 0;

            string[] TipeDatadata =
            {
                "по таблицам СП",
                "по лабораторным испытаниям"
            };
            TipeData.Items.AddRange(TipeDatadata);
            TipeData.SelectedIndex = 0;
        }

        // === ЗАПОЛНЕНИЕ ТАБЛИЦЫ КОЭФФИЦИЕНТОВ Yc2 ===
        private void InitCoeffTable()
        {
            coeffTableYc2 = new Dictionary<int, List<RangeCoeff>>();

            coeffTableYc2[0] = new List<RangeCoeff>
            {
                new RangeCoeff(0.0, 1.5, 1.4),
                new RangeCoeff(1.5, 2.0, 1.36),
                new RangeCoeff(2.0, 2.5, 1.32),
                new RangeCoeff(2.5, 3.0, 1.28),
                new RangeCoeff(3.0, 3.5, 1.24),
                new RangeCoeff(3.5, 4.0, 1.22),
                new RangeCoeff(4.0, 100.0, 1.2)
            };

            coeffTableYc2[1] = new List<RangeCoeff>
            {
                new RangeCoeff(0.0, 1.5, 1.3),
                new RangeCoeff(1.5, 2.0, 1.26),
                new RangeCoeff(2.0, 2.5, 1.22),
                new RangeCoeff(2.5, 3.0, 1.18),
                new RangeCoeff(3.0, 3.5, 1.14),
                new RangeCoeff(3.5, 4.0, 1.12),
                new RangeCoeff(4.0, 100.0, 1.1)
            };

            coeffTableYc2[2] = new List<RangeCoeff>
            {
                new RangeCoeff(0.0, 1.5, 1.2),
                new RangeCoeff(1.5, 2.0, 1.16),
                new RangeCoeff(2.0, 2.5, 1.12),
                new RangeCoeff(2.5, 3.0, 1.08),
                new RangeCoeff(3.0, 3.5, 1.04),
                new RangeCoeff(3.5, 4.0, 1.02),
                new RangeCoeff(4.0, 100.0, 1.0)
            };

            coeffTableYc2[3] = new List<RangeCoeff>
            {
                new RangeCoeff(0.0, 1.5, 1.1),
                new RangeCoeff(1.5, 2.0, 1.08),
                new RangeCoeff(2.0, 2.5, 1.06),
                new RangeCoeff(2.5, 3.0, 1.04),
                new RangeCoeff(3.0, 3.5, 1.02),
                new RangeCoeff(3.5, 4.0, 1.01),
                new RangeCoeff(4.0, 100.0, 1.0)
            };

            coeffTableYc2[4] = new List<RangeCoeff>
            {
                new RangeCoeff(0.0, 1.5, 1.0),
                new RangeCoeff(1.5, 2.0, 1.0),
                new RangeCoeff(2.0, 2.5, 1.0),
                new RangeCoeff(2.5, 3.0, 1.0),
                new RangeCoeff(3.0, 3.5, 1.0),
                new RangeCoeff(3.5, 4.0, 1.0),
                new RangeCoeff(4.0, 100.0, 1.0)
            };

            coeffTableYc2[5] = new List<RangeCoeff>
            {
                new RangeCoeff(0.0, 1.5, 1.0),
                new RangeCoeff(1.5, 2.0, 1.0),
                new RangeCoeff(2.0, 2.5, 1.0),
                new RangeCoeff(2.5, 3.0, 1.0),
                new RangeCoeff(3.0, 3.5, 1.0),
                new RangeCoeff(3.5, 4.0, 1.0),
                new RangeCoeff(4.0, 100.0, 1.0)
            };
        }

        // === ПОИСК КОЭФФИЦИЕНТА Yc2 ===
        private double GetCoefficientYc2(int index, double depth)
        {
            if (!coeffTableYc2.ContainsKey(index))
                return 1.0;

            foreach (RangeCoeff range in coeffTableYc2[index])
            {
                if (depth > range.Min && depth <= range.Max)
                    return range.Coeff;
            }
            return 1.0;
        }

        // === ПОИСК КОЭФФИЦИЕНТА Yc1 ===
        private double GetCoefficientYc1(int index)
        {
            Dictionary<int, double> Yc1key = new Dictionary<int, double>()
            {
                { 0, 1.4 },
                { 1, 1.3 },
                { 2, 1.1 },
                { 3, 1.25 },
                { 4, 1.2 },
                { 5, 1.1 }
            };

            if (Yc1key.ContainsKey(index))
                return Yc1key[index];
            else
                return 1.0;
        }

        // === ЗАПОЛНЕНИЕ ТАБЛИЦЫ myTable1 ===
        private void InitTable()
        {
            myTable1 = new Dictionary<int, (double, double, double)>
            {
                { 0, (0, 1, 3.14) },
                { 1, (0.01, 1.06, 3.23) },
                { 2, (0.03, 1.12, 3.32) },
                { 3, (0.04, 1.18, 3.41) },
                { 4, (0.06, 1.25, 3.51) },
                { 5, (0.08, 1.32, 3.61) },
                { 6, (0.1, 1.39, 3.71) },
                { 7, (0.12, 1.47, 3.82) },
                { 8, (0.14, 1.55, 3.93) },
                { 9, (0.16, 1.64, 4.05) },
                { 10, (0.18, 1.73, 4.17) },
                { 11, (0.21, 1.83, 4.29) },
                { 12, (0.23, 1.94, 4.42) },
                { 13, (0.26, 2.05, 4.55) },
                { 14, (0.29, 2.17, 4.69) },
                { 15, (0.32, 2.3, 4.84) },
                { 16, (0.36, 2.43, 4.99) },
                { 17, (0.39, 2.57, 5.15) },
                { 18, (0.43, 2.73, 5.31) },
                { 19, (0.47, 2.89, 5.48) },
                { 20, (0.51, 3.06, 5.66) },
                { 21, (0.56, 3.24, 5.84) },
                { 22, (0.61, 3.44, 6.04) },
                { 23, (0.66, 3.65, 6.24) },
                { 24, (0.72, 3.87, 6.45) },
                { 25, (0.78, 4.11, 6.67) },
                { 26, (0.84, 4.37, 6.9) },
                { 27, (0.91, 4.64, 7.14) },
                { 28, (0.98, 4.93, 7.4) },
                { 29, (1.06, 5.25, 7.67) },
                { 30, (1.15, 5.59, 7.95) },
                { 31, (1.24, 5.95, 8.24) },
                { 32, (1.34, 6.34, 8.55) },
                { 33, (1.44, 6.76, 8.88) },
                { 34, (1.55, 7.22, 9.22) },
                { 35, (1.68, 7.71, 9.58) },
                { 36, (1.81, 8.24, 9.97) },
                { 37, (1.95, 8.81, 10.37) },
                { 38, (2.11, 9.44, 10.8) },
                { 39, (2.28, 10.11, 11.25) },
                { 40, (2.46, 10.85, 11.73) },
                { 41, (2.66, 11.64, 12.24) },
                { 42, (2.88, 12.51, 12.79) },
                { 43, (3.12, 13.46, 13.37) },
                { 44, (3.38, 14.5, 13.98) },
                { 45, (3.66, 15.64, 14.64) }
            };
        }

        // === ПОЛУЧЕНИЕ ЗНАЧЕНИЙ ИЗ ТАБЛИЦЫ ===
        private (double My, double Mq, double Mc) GetTableValues(int key)
        {
            if (myTable1.ContainsKey(key))
            {
                var values = myTable1[key];
                return (values.a, values.b, values.c);
            }
            return (0, 0, 0);
        }

        // === ОСНОВНАЯ КНОПКА РАСЧЁТА ===
        private void StartR_Click(object sender, EventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(Ugol.Text) ||
                string.IsNullOrEmpty(Sceplenie.Text) ||
                string.IsNullOrEmpty(VesNige.Text) ||
                string.IsNullOrEmpty(VesVishe.Text) ||
                string.IsNullOrEmpty(Glubina.Text) ||
                string.IsNullOrEmpty(DlinaP.Text) ||
                string.IsNullOrEmpty(VisotaP.Text) ||
                string.IsNullOrEmpty(ShirinaP.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Парсинг данных
            int u = int.Parse(Ugol.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double SC = double.Parse(Sceplenie.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double yn = double.Parse(VesNige.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double yv = double.Parse(VesVishe.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double h = double.Parse(Glubina.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double a = double.Parse(DlinaP.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double b = double.Parse(VisotaP.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double c = double.Parse(ShirinaP.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            

            // Проверка на нулевые значения
            if (yn == 0 || yv == 0 || h == 0 || a == 0 || b == 0 || c == 0)
            {
                MessageBox.Show("Значения не могут быть нулевыми!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Проверка на угол внутреннего трения
            if (u < 0 || u > 45)
            {
                MessageBox.Show("Угол должен быть от 0 до 45 градусов", "Ошибка");
                return;
            }

            // Получаем индекс типа грунта
            int index = TipeGround.SelectedIndex;
            int index1= TipeData.SelectedIndex;

            // Получаем коэффициенты
            double Yc1 = GetCoefficientYc1(index);
            double lh = a / b;
            double Yc2 = GetCoefficientYc2(index, lh);

            // Получаем коэффициент К
            if (index1 == 0) { K = 1.1; }
            else if (index1 == 1) { K = 1.0; }

            // Получаем значения из словаря по индексу (или по другому ключу, если нужно)
            var tableValues = GetTableValues(u);
            double My = tableValues.My;
            double Mq = tableValues.Mq;
            double Mc = tableValues.Mc;

            if (My == 0 && Mq == 0 && Mc == 0)
            {
                MessageBox.Show("Индекс не найден в таблице!", "Ошибка");
                return;
            }

            // Расчёт
            double Kz = (c < 10) ? 1.0 : 0.2 + 8 / c;

            double R = Math.Round((Yc1 * Yc2 / K) * (My * Kz * c * yn + Mq * h * yv + Mc * SC),1);

            double x1 = (Yc1 * Yc2 / K);
            double x2 = (My * Kz * c * yn);
            double x3 = Mq * h * yv;
            double x4 = Mc * SC;
            A1.Text =a.ToString();
            B1.Text =c.ToString();

            // Вывод результата с расшифровкой
            Result.Text=R.ToString();
            Vdali.Text = R.ToString();
            string result =
                "═══════════════════════════════════════\n" +
                "📊 РЕЗУЛЬТАТЫ РАСЧЁТА\n" +
                "═══════════════════════════════════════\n" +
                $"Длина плиты A        = {a:F2} (м)\n" +
                $"Ширина плиты B       = {c:F2} (м)\n" +
                $"Толщина плиты t      = {b:F2} (м)\n" +
                $"Глубина заложения H = {h:F2} (м)\n" +
                $"Грунт в основании принят = {TipeGround.SelectedItem}\n" +
                $"Данные физ.мех свойств приняты = {TipeData.SelectedItem}\n" +
                $"Удельный вес грунта ниже подошвы yn  = {yn:F2} т/м3\n" +
                $"Удельный вес грунта выше подошвы yv = {yv:F2} т/м3\n" +
                $"Сцепление SC = {SC:F2} т/м2\n" +
                $"Угол внутреннего трения = {u:F2} град.\n" +
                $"───────────────────────────────────────\n" +
                $"Значение коэффициента Yc1 = {Yc1:F2}\n" +
                $"Соотношение L/H   = {lh:F2}\n" +
                $"Значение коэффициента Yc2 = {Yc2:F2}\n" +
                $"Значение коэффициента K   = {K:F2}\n" +
                $"Значение коэффициента Kz   = {Kz:F2}\n" +
                $"My  = {My:F2}\n" +
                $"Mq  = {Mq:F2}\n" +
                $"Mc  = {Mc:F2}\n" +
                $"───────────────────────────────────────\n" +
                $"R = (Yc1 * Yc2 / K) * (My * Kz * B * yn + Mq * h * yv + Mc * SC)\n" +
                $"───────────────────────────────────────\n" +
                $"R = {x1:F2} * ({x2:F2} + {x3:F2} + {x4:F2}) = {R:F2} т/м2\n" +
                "═══════════════════════════════════════";
            // Сохраняем отчёт в файл
            try
            {
                string filePath = "Отчёт_расчёта.txt";
                System.IO.File.WriteAllText(filePath, result, Encoding.UTF8);
                MessageBox.Show(result + "\n\n✅ Отчёт сохранён в файл:\n" + filePath, "Результат расчёта");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Проверка на пустые поля
            if (string.IsNullOrEmpty(VerticalN.Text) ||
                string.IsNullOrEmpty(Moment.Text) ||
                string.IsNullOrEmpty(Nadegnost.Text) ||
                string.IsNullOrEmpty(Kuslov.Text) ||
                string.IsNullOrEmpty(Vdali.Text) ||
                string.IsNullOrEmpty(A1.Text) ||
                string.IsNullOrEmpty(B1.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Парсим
            double N = double.Parse(VerticalN.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double M = double.Parse(Moment.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double Kn = double.Parse(Nadegnost.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double Ku = double.Parse(Kuslov.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double R = double.Parse(Vdali.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double a = double.Parse(A1.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            double b = double.Parse(B1.Text.Replace(',', '.'), CultureInfo.InvariantCulture);
            // Проверяем
            if (Kn == 0 || Ku == 0)
            {
                MessageBox.Show("Значения не могут быть нулевыми! Проверьте ввод.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Расчёт

            double Psr = Math.Round(N / (a*b), 1);

            double Rrasc = Math.Round(R / Kn, 1);

            double Pmax = Math.Round(Psr + ((6 * M) / (a * b * b)),1);

            double Rrascmin=Math.Round(R*Ku/Kn,1);


            Rmin.Text= Rrascmin.ToString();
            PmaxIM.Text= Pmax.ToString();
            Srednee.Text = Psr.ToString();
            Rsr.Text = Rrasc.ToString();

            if (Psr > Rrasc)
                Uslovie1.Text = "Условие НЕ ВЫПОЛНЯЕТСЯ";
            else
                Uslovie1.Text = "Условие ВЫПОЛНЯЕТСЯ";

            if (Pmax>Rrascmin)
                Uslovie2.Text = "Условие НЕ ВЫПОЛНЯЕТСЯ";
            else 
                Uslovie2.Text = "Условие ВЫПОЛНЯЕТСЯ";

        }
        


            
            
           

    }
}





