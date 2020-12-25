using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Tree_Of_Objects_And_Subscription
{
    public partial class Form1 : Form
    {
        MVC model;
        public Form1()
        {
            InitializeComponent();
            model = new MVC();
            model.observers += new System.EventHandler(this.UpdateFromMVC);
            storage.RegisterObserver(tree);
        }
        #region Классы фигур
        public class Shape // Родительский (класс) фигуры
        {
            public int x, y;
            public int radius = 30, size = 60;
            public int X_min, X_max, Y_min, Y_max;
            protected Color color = default_color;
            private bool is_selected = false;
            private bool is_sticky = false;
            public Shape()
            {
            }
            public virtual void SetColor(Color color)
            {
                this.color = color;
            }
            public Color GetColor()
            {
                return color;
            }
            public void Select(bool is_selected)
            {
                this.is_selected = is_selected;
            }
            public bool IsSelected()
            {
                return is_selected;
            }
            public void Stick(bool is_sticky)
            {
                this.is_sticky = is_sticky;
            }
            public bool IsSticky()
            {
                return is_sticky;
            }
            public virtual void Draw_Shape(Pen pen, Brush solidBrush, Panel Canvas_Panel) { }
            public virtual void Move_x(int x, Panel Canvas_Panel) { }
            public virtual void Move_y(int y, Panel Canvas_Panel) { }
            public virtual void Change_Size(int size) { }
            public virtual bool Check_Shape(int x, int y) { return false; }
            public virtual string Save() { return ""; }
            public virtual void Load(string x, string y, string c, string color) { }
            public virtual void Load(ref StreamReader sr, Shape Shape, CreateShape createShape) { }
            public virtual void Add_to_Group(ref Shape shape) { }
            public virtual void Ungroup(ref Storage storage, int c) { }
            public virtual void Choice(ref StreamReader sr, ref Shape shape, CreateShape createShape) { }
            public virtual int getX_min() { return 0; }
            public virtual int getX_max() { return 0; }
            public virtual int getY_min() { return 0; }
            public virtual int getY_max() { return 0; }
            public virtual string Name() { return ""; }
        };
        public class CreateShape : Shape
        {
            public override void Choice(ref StreamReader sr, ref Shape shape, CreateShape createShape)
            {
                string str = sr.ReadLine();
                switch (str)
                {   // В зависимости какая фигура выбрана
                    case "Circle":
                        shape = new Circle();
                        shape.Load(sr.ReadLine(), sr.ReadLine(), sr.ReadLine(), sr.ReadLine());
                        break;
                    case "Triangle":
                        shape = new Triangle();
                        shape.Load(sr.ReadLine(), sr.ReadLine(), sr.ReadLine(), sr.ReadLine());
                        break;
                    case "Square":
                        shape = new Square();
                        shape.Load(sr.ReadLine(), sr.ReadLine(), sr.ReadLine(), sr.ReadLine());
                        break;
                    case "Group":
                        shape = new Group();
                        shape.Load(ref sr, shape, createShape);
                        break;
                }
            }
        }
        class Circle : Shape// Класс круга
        {
            //public int radius = 30;
            public Circle() { }
            public Circle(int x, int y)
            {
                this.x = x - radius;
                this.y = y - radius;
            }
            public override void Draw_Shape(Pen pen, Brush solidBrush, Panel Canvas_Panel)
            {
                Canvas_Panel.CreateGraphics().DrawEllipse(
                    pen, x, y, radius * 2, radius * 2);
                Canvas_Panel.CreateGraphics().FillEllipse(
                    solidBrush, x, y, radius * 2, radius * 2);
            }
            public override void Move_x(int x, Panel Canvas_Panel)
            {
                int c = this.x + x;
                int limit = Canvas_Panel.ClientSize.Width - (radius * 2);
                Check_borders(c, x, limit, limit - 2, ref this.x);
            }
            public override void Move_y(int y, Panel Canvas_Panel)
            {
                int c = this.y + y;
                int limit = Canvas_Panel.ClientSize.Height - (radius * 2);
                Check_borders(c, y, limit, limit - 2, ref this.y);
            }
            public override void Change_Size(int size)
            {
                radius += size;
            }
            public override bool Check_Shape(int x, int y)
            {
                return ((x - this.x - radius) * (x - this.x - radius) +
                        (y - this.y - radius) * (y - this.y - radius)) <= (radius * radius);
            }
            public override string Save()
            {
                return "Circle" + "\n" + x + "\n" + y + "\n" + radius + "\n" + GetColor().ToArgb().ToString();
            }
            public override void Load(string x, string y, string radius, string color)
            {
                this.x = Convert.ToInt32(x);
                this.y = Convert.ToInt32(y);
                this.radius = Convert.ToInt32(radius);
                SetColor(Color.FromArgb(Convert.ToInt32(color)));
            }
            public override int getX_min() { return x; }
            public override int getX_max() { return x + 2 * radius; }
            public override int getY_min() { return y; }
            public override int getY_max() { return y + 2 * radius; }
            public override string Name() { return "Circle"; }
        }
        class Triangle : Shape // Класс треугольника
        {
            //public int size = 60;
            public Triangle() { }
            public Triangle(int x, int y) // Конструктор с параметрами
            {
                this.x = x - size / 2;
                this.y = y - size / 2;
            }
            public override void Draw_Shape(Pen pen, Brush solidBrush, Panel Canvas_Panel)
            {
                Point[] points = new Point[3]; // Массив точек для прорисовки треугольника
                points[0].X = this.x + size / 2; points[0].Y = this.y;
                points[1].X = this.x; points[1].Y = this.y + size;
                points[2].X = this.x + size; points[2].Y = this.y + size;
                Canvas_Panel.CreateGraphics().DrawPolygon(pen, points);
                Canvas_Panel.CreateGraphics().FillPolygon(solidBrush, points);
            }
            public override void Move_x(int x, Panel Canvas_Panel)
            {
                int t = this.x + x;
                int limit = Canvas_Panel.ClientSize.Width - size;
                Check_borders(t, x, limit, limit - 2, ref this.x);
            }
            public override void Move_y(int y, Panel Canvas_Panel)
            {
                int t = this.y + y;
                int limit = Canvas_Panel.ClientSize.Height - size;
                Check_borders(t, y, limit, limit - 2, ref this.y);
            }
            public override void Change_Size(int size)
            {
                this.size += size;
            }
            public override bool Check_Shape(int x, int y)
            {
                Point[] points = new Point[3]; // Массив точек для прорисовки треугольника
                points[0].X = this.x + size / 2; points[0].Y = this.y;
                points[1].X = this.x; points[1].Y = this.y + size;
                points[2].X = this.x + size; points[2].Y = this.y + size;
                int a = (points[0].X - x) * (points[1].Y - points[0].Y) - (points[1].X - points[0].X) * (points[0].Y - y);
                int b = (points[1].X - x) * (points[2].Y - points[1].Y) - (points[2].X - points[1].X) * (points[1].Y - y);
                int c = (points[2].X - x) * (points[0].Y - points[2].Y) - (points[0].X - points[2].X) * (points[2].Y - y);
                return ((a > 0 && b > 0 && c > 0) || (a < 0 && b < 0 && c < 0));
            }
            public override string Save()
            {
                return "Triangle" + "\n" + x + "\n" + y + "\n" + size + "\n" + GetColor().ToArgb().ToString();
            }
            public override void Load(string x, string y, string size, string color)
            {
                this.x = Convert.ToInt32(x);
                this.y = Convert.ToInt32(y);
                this.size = Convert.ToInt32(size);
                SetColor(Color.FromArgb(Convert.ToInt32(color)));
            }
            public override int getX_min() { return x; }
            public override int getX_max() { return x + size; }
            public override int getY_min() { return y; }
            public override int getY_max() { return y + size; }
            public override string Name() { return "Triangle"; }
        }
        class Square : Shape // Класс квадрата
        {
            //public int size = 60;
            public Square() { }
            public Square(int x, int y)
            {
                this.x = x - size / 2;
                this.y = y - size / 2;
            }
            public override void Draw_Shape(Pen pen, Brush solidBrush, Panel Canvas_Panel)
            {
                Canvas_Panel.CreateGraphics().DrawRectangle(pen,
                    x, y, size, size);
                Canvas_Panel.CreateGraphics().FillRectangle(solidBrush,
                    x, y, size, size);
            }
            public override void Move_x(int x, Panel Canvas_Panel)
            {
                int s = this.x + x;
                int limit = Canvas_Panel.ClientSize.Width - size;
                Check_borders(s, x, limit, --limit, ref this.x);
            }
            public override void Move_y(int y, Panel Canvas_Panel)
            {
                int s = this.y + y;
                int limit = Canvas_Panel.ClientSize.Height - size;
                Check_borders(s, y, limit, --limit, ref this.y);
            }
            public override void Change_Size(int size)
            {
                this.size += size;
            }
            public override bool Check_Shape(int x, int y)
            {
                return (this.x <= x && x <= (this.x + size) &&
                        this.y <= y && y <= (this.y + size));
            }
            public override string Save()
            {
                return "Square" + "\n" + x + "\n" + y + "\n" + size + "\n" + GetColor().ToArgb().ToString();
            }
            public override void Load(string x, string y, string size, string color)
            {
                this.x = Convert.ToInt32(x);
                this.y = Convert.ToInt32(y);
                this.size = Convert.ToInt32(size);
                SetColor(Color.FromArgb(Convert.ToInt32(color)));
            }
            public override int getX_min() { return x; }
            public override int getX_max() { return x + size; }
            public override int getY_min() { return y; }
            public override int getY_max() { return y + size; }
            public override string Name() { return "Square"; }
        }
        class Group : Shape
        {
            public int maxcount = 10;
            public Shape[] group;
            public int count;
            public Group()
            {   // Выделяем maxcount мест в хранилище
                count = 0;
                group = new Shape[maxcount];
                for (int i = 0; i < maxcount; ++i)
                    group[i] = null;
            }
            public override string Save()
            {
                string str = "Group" + "\n" + count;
                for (int i = 0; i < count; ++i)
                    str += "\n" + group[i].Save();
                return str;
            }
            public override void Load(ref StreamReader sr, Shape shape, CreateShape createShape)
            {
                int chislo = Convert.ToInt32(sr.ReadLine());
                for (int i = 0; i < chislo; ++i)
                {
                    createShape.Choice(ref sr, ref shape, createShape);
                    Add_to_Group(ref shape);
                }
            }
            public override void Add_to_Group(ref Shape shape)
            {
                if (count >= maxcount)
                    return;
                group[count] = shape;
                ++count;
            }
            public override void Ungroup(ref Storage storage, int c)
            {
                storage.Delete_object(c);
                storage.Sort(count_cells);
                for (int i = 0; i < count; ++i)
                {
                    storage.Add_object(count_elements, ref group[i], count_cells, ref indexin);
                }
            }
            public override void Draw_Shape(Pen pen, Brush solidBrush, Panel Canvas_Panel)
            {
                for (int i = 0; i < count; ++i)
                {
                    pen.Color = group[i].GetColor();
                    group[i].Draw_Shape(pen, solidBrush, Canvas_Panel);
                }
            }
            public void Group_Borders()
            {
                X_min = int.MaxValue; X_max = 0; Y_min = int.MaxValue; Y_max = 0;
                for (int i = 0; i < count; ++i)
                {
                    int f = 0;
                    f = group[i].getX_min();
                    if (f < X_min)
                        X_min = f;
                    f = group[i].getX_max();
                    if (f > X_max)
                        X_max = f;
                    f = group[i].getY_min();
                    if (f < Y_min)
                        Y_min = f;
                    f = group[i].getY_max();
                    if (f > Y_max)
                        Y_max = f;
                }
            }
            public override void Move_x(int x, Panel Canvas_Panel)
            {
                Group_Borders();
                if ((X_min + x) > 0 && (X_max + x) < Canvas_Panel.ClientSize.Width)
                    for (int i = 0; i < count; ++i)
                        group[i].Move_x(x, Canvas_Panel);
            }
            public override void Move_y(int y, Panel Canvas_Panel)
            {
                Group_Borders();
                if ((Y_min + y) > 0 && (Y_max + y) < Canvas_Panel.ClientSize.Height)
                    for (int i = 0; i < count; ++i)
                        group[i].Move_y(y, Canvas_Panel);
            }
            public override void Change_Size(int size)
            {
                for (int i = 0; i < count; ++i)
                {
                    group[i].Change_Size(size);
                }
            }
            public override bool Check_Shape(int x, int y)
            {
                for (int i = 0; i < count; ++i)
                {
                    if (group[i].Check_Shape(x, y))
                        return true;
                }
                return false;
            }
            public override void SetColor(Color color)
            {
                for (int i = 0; i < count; ++i)
                {
                    group[i].SetColor(color);
                }
            }
            public override string Name() { return "Group"; }
        }
        #endregion
        public interface IObservable
        {   // Наблюдаемый объект
            void RegisterObserver(IObserver o);
            void RemoveObserver(IObserver o);
            void NotifyObservers();
        }
        public interface IObserver
        {   // Наблюдатель
            void Update(ref TreeView treeView, Storage storage);
        }
        public class Storage : IObservable
        {
            public Shape[] objects;
            public TreeView treeView;
            public List<IObserver> observers;
            public Storage(int amount)
            {   // Конструктор по умолчанию 
                objects = new Shape[amount];
                observers = new List<IObserver>();
                for (int i = 0; i < amount; ++i)
                    objects[i] = null;
            }
            public void Initialization(int amount)
            {   // Выделяем amount мест в хранилище
                objects = new Shape[amount];
                for (int i = 0; i < amount; ++i)
                    objects[i] = null;
            }
            public void Initialization_tree(ref TreeView treeView)
            {
                this.treeView = treeView;
            }
            public void Add_object(int ind, ref Shape new_object, int count, ref int indexin)
            {   // Добавляет ячейку в хранилище
                // Если ячейка занята ищем свободное место
                while (objects[ind] != null)
                {
                    ind = (ind + 1) % count;
                }
                objects[ind] = new_object;
                indexin = ind;
                Sort(count_cells);
                NotifyObservers();
            }
            public void Delete_object(int ind)
            {   // Удаляет объект из хранилища
                objects[ind] = null;
                if (count_elements > 0)
                    count_elements--;
                NotifyObservers();
            }
            public void Sort(int size)
            {
                Storage storage1 = new Storage(size);
                int col = 0;
                for (int i = 0; i < size; ++i)
                {
                    if (!Is_empty(i))
                    {
                        storage1.objects[col] = objects[i];
                        ++col;
                    }
                }
                Initialization(size);
                for (int i = 0; i < size; ++i)
                    objects[i] = storage1.objects[i];
            }
            public bool Is_empty(int ind)
            {   // Проверяет занято ли место в хранилище
                if (objects[ind] == null)
                    return true;
                else return false;
            }
            public int Occupied(int size)
            { // Определяет кол-во занятых мест в хранилище
                int count_occupied = 0;
                for (int i = 0; i < size; ++i)
                    if (!Is_empty(i))
                        ++count_occupied;
                return count_occupied;
            }
            public void Increase_Storage(ref int size)
            { // Увеличивает хранилище в 2 раза
                Storage new_storage = new Storage(size * 2);
                for (int i = 0; i < size; ++i)
                    new_storage.objects[i] = objects[i];
                for (int i = size; i < (size * 2) - 1; ++i)
                    new_storage.objects[i] = null;
                size *= 2;
                Initialization(size);
                for (int i = 0; i < size; ++i)
                    objects[i] = new_storage.objects[i];
            }
            public void RegisterObserver(IObserver o)
            {
                observers.Add(o);
            }
            public void RemoveObserver(IObserver o)
            {
                observers.Remove(o);
            }
            public void NotifyObservers()
            {
                foreach (IObserver observer in observers)
                    observer.Update(ref treeView, this);
            }
            ~Storage() { }
        };
        public class TreeViews : IObserver
        {
            public TreeViews() { }
            public void Update(ref TreeView treeView, Storage storage)
            {
                treeView.Nodes.Clear();
                treeView.Nodes.Add("Фигуры");
                for (int i = 0; i < count_cells; ++i)
                {
                    if (!storage.Is_empty(i))
                    {
                        FillNode(treeView.Nodes[0], storage.objects[i]);
                    }
                }
                treeView.ExpandAll();
            }
            public void TreeSelect(ref TreeView treeView, int index) //выбор узла
            {   // Выделяем узел
                treeView.SelectedNode = treeView.Nodes[0].Nodes[index];
                treeView.Focus();
            }
            public void FillNode(TreeNode treeNode, Shape shape)
            {
                TreeNode nodes = treeNode.Nodes.Add(shape.Name());
                if (shape.Name() == "Group")
                {
                    for (int i = 0; i < (shape as Group).count; ++i)
                    {
                        FillNode(nodes, (shape as Group).group[i]);
                    }
                }
            }
        }
        public class StickyObserver: IObserver
        {
            public StickyObserver() { }
            public bool checkCircle(Storage stg, int i, int j)
            {
                if ((stg.objects[j].x - stg.objects[i].x) * (stg.objects[j].x - stg.objects[i].x) +
                    (stg.objects[j].y - stg.objects[i].y) * (stg.objects[j].y - stg.objects[i].y)
                    <= (stg.objects[i].radius + stg.objects[j].radius) * (stg.objects[i].radius + stg.objects[j].radius) + 1)
                    return true;
                else return false;
            }
            //public bool checkLine(Storage stg, int i, int j)
            //{
            //    if (stg.objects[i].x + stg.objects[i].lenght >= stg.objects[j].x - stg.objects[j].lenght
            //        && stg.objects[i].x - stg.objects[i].lenght <= stg.objects[j].x + stg.objects[j].lenght
            //        && stg.objects[i].y >= stg.objects[j].y - 10
            //        && stg.objects[i].y <= stg.objects[j].y + 10)
            //        return true;
            //    else return false;
            //}
            public bool checkSquare(Storage stg, int i, int j)
            {
                if (stg.objects[i].x + (stg.objects[i].size / 2) >= stg.objects[j].x - (stg.objects[j].size / 2)
                    && stg.objects[i].x - (stg.objects[i].size / 2) <= stg.objects[j].x + (stg.objects[j].size / 2)
                    && stg.objects[i].y >= stg.objects[j].y - (stg.objects[j].size)
                    && stg.objects[i].y <= stg.objects[j].y + (stg.objects[j].size))
                    return true;
                else return false;
            }

            public bool FigureCheck(Storage stg, int i, int j, string b, int d)
            {
                string h;
                if (d == 1)
                {
                    h = b;
                }
                else h = stg.objects[j].Name();
                switch (h)
                {
                    case "Circle":
                        if (checkCircle(stg, i, j))
                            return true;
                        break;

                    //case "Line":
                    //    if (checkLine(stg, i, j))
                    //        return true;
                    //    break;

                    case "Square":
                        if (checkSquare(stg, i, j))
                            return true;
                        break;
                    case "Group":
                        for (int v = 0; v < (stg.objects[j] as Group).count; ++v)
                        {
                            string l = (stg.objects[j] as Group).Name();
                            if (FigureCheck(stg, i, v, l, 1))
                                return true;
                        }
                        break;
                    case null:
                        return false;




                }
                return false;

            }
            public void Update(ref TreeView treeView, Storage stg)
            {
                int p = 0;
                for (int i = 0; i < count_cells; ++i)
                {
                    if (!stg.Is_empty(i))
                    {
                        if (stg.objects[i].IsSticky() == true)
                        {
                            p = i;
                            break;
                        }
                    }
                }
                for (int i = 0; i < count_cells; ++i)
                {
                    if (!stg.Is_empty(i))
                    {
                        if (p == i)
                        {
                            continue;
                        }
                        string f = "";
                        if (FigureCheck(stg, p, i, f, 0))
                        {
                            stg.objects[i].Select(true);
                        }
                    }
                }
            }



            //public void Update(ref TreeView treeView, Storage storage)
            //{
            //    int x = 0, y = 0;
            //    for (int i = 0; i < count_cells; ++i)
            //        if (!storage.Is_empty(i))
            //            if (storage.objects[i].IsSelected() && storage.objects[i].IsSticky())
            //            {
            //                x = storage.objects[i].x;
            //                y = storage.objects[i].y;
            //            }
            //    for (int i = 0; i < count_cells; ++i)
            //        if (!storage.Is_empty(i))
            //            if (storage.objects[i].Check_Shape(x, y))
            //                storage.objects[i].Select(true);
            //}
        }
        public class MVC
        {
            private string figure;
            public System.EventHandler observers;
            public void setFigure(string figure)
            {

                this.figure = figure;
                observers.Invoke(this, null);
            }
            public string getFigure()
            {
                return figure;
            }
        }
        #region Объявление переменных
        TreeViews tree = new TreeViews();
        string figure_now; // Хранит значение нынешней фигуры
        static int count_cells = 5; // Кол-во ячеек в хранилище
        static int indexin = 0; // Индекс, в какое место был помещён круг
        static int count_elements = 0; // Кол-во элементов в хранилище
        Storage storage = new Storage(count_cells); // Хранилище объектов
        static Color default_color = Color.Black; // Цвет по умолчанию
        Shape shape = new Shape(); // Объект класса Shape
        #endregion
        #region Проверки
        private int Check_Shape(ref Storage storage, int x, int y, int start)
        {
            // Проверяет есть ли уже фигура с такими же координатами в хранилище
            if (storage.Occupied(count_cells) != 0)
            {
                for (int i = start; i < count_cells; ++i)
                {
                    if (!storage.Is_empty(i))
                    {   // Если под i индексом в хранилище есть объект
                        if (storage.objects[i].Check_Shape(x, y))
                            return i;
                    }
                }
            }
            return -1;
        }
        static public void Check_borders(int f, int number, int limit1, int limit2, ref int coord)
        {   // Проверка на выход фигуры за границы
            if (f > 0 && f < limit1)
                coord += number;
            else
            {
                if (f <= 0)
                    coord = 1;
                else
                    if (f >= limit2)
                    coord = limit2;
            }
        }
        #endregion
        #region Манипуляции с фигурами
        private void Move_y(ref Storage storage, int y)
        {   // Функция для перемещения фигур по оси Y
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())// Если объект выделен
                    {
                        storage.objects[i].Move_y(y, Canvas_Panel);
                        storage.NotifyObservers();
                    }
        }
        private void Move_x(ref Storage storage, int x)
        {   // Функция для перемещения фигур по оси X
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())// Если объект выделен
                    {
                        storage.objects[i].Move_x(x, Canvas_Panel);
                        storage.NotifyObservers();
                    }
        }
        private void Change_Size(ref Storage storage, int size)
        {   // Увеличивает или уменьшает размер фигур, в зависимости от size
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))// Если под i индексом в хранилище есть объект
                    if (storage.objects[i].IsSelected())
                        storage.objects[i].Change_Size(size);
        }
        #endregion
        #region Обработчики Событий
        private void Canvas_Panel_MouseDown(object sender, MouseEventArgs e)//Обработчик нажатия на полотно
        {
            int ind = Check_Shape(ref storage, e.X, e.Y, 0);
            storage.Initialization_tree(ref TreeView);
            if (e.Button == MouseButtons.Left)
            {
                if (count_elements == count_cells)
                    // Увеличиваем хранилище
                    storage.Increase_Storage(ref count_cells);

                switch (figure_now)//Узнаем какая сейчас выбрана фигура
                {
                    case "Круг":
                        shape = new Circle(e.X, e.Y);
                        break;
                    case "Треугольник":
                        shape = new Triangle(e.X, e.Y);
                        break;
                    case "Квадрат":
                        shape = new Square(e.X, e.Y);
                        break;
                }
                if (ind == -1)//Если место свободно
                {
                    Deselect(ref storage);//Убираем выделение у всех объектов
                    storage.Add_object(count_elements, ref shape, count_cells, ref indexin);//Добавляем объект в хранилище
                    shape.Select(true);//Объект выделяем
                    Redraw_Shapes(ref storage);//Перерисовываем все объекты
                    count_elements++;//Увеличиваем кол-во элементов
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                if (ind != -1)//Если там есть фигуры
                {
                    if (Control.ModifierKeys == Keys.Control)
                    {   // Если нажат ctrl, то выделяем несколько объектов
                        // Вызываем функцию отрисовки фигуры  
                        storage.objects[ind].Select(true);
                        Draw_Shape(ref storage, ind);
                    }
                    else
                    {   // Иначе выделяем только один объект
                        // Снимаем выделение у всех объектов хранилища
                        Deselect(ref storage);
                        Draw_Shape(ref storage, ind);
                        tree.TreeSelect(ref TreeView, ind);
                    }
                    return;
                }
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)//Обработчик нажатия на клавиши
        {
            if (e.KeyCode == Keys.Delete)   //Если нажили на Delete
                Remove_Selected(ref storage);
            if (e.KeyCode == Keys.Add)      //Если +
                Change_Size(ref storage, 5);
            if (e.KeyCode == Keys.Subtract) //Если -
                Change_Size(ref storage, -5);
            if (e.KeyCode == Keys.W)
                Move_y(ref storage, -5);
            if (e.KeyCode == Keys.S)
                Move_y(ref storage, 5);
            if (e.KeyCode == Keys.A)
                Move_x(ref storage, -5);
            if (e.KeyCode == Keys.D)
                Move_x(ref storage, 5);
            Redraw_Shapes(ref storage);
        }
        private void Color_ToolStripButton_Click(object sender, EventArgs e)
        {
            if (ColorDialog.ShowDialog() == DialogResult.Cancel)
                return;
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())
                    {
                        storage.objects[i].SetColor(ColorDialog.Color);
                        Redraw_Shapes(ref storage);
                    }
        }
        private void Circle_ToolStripButton_Click(object sender, EventArgs e)
        {
            figure_now = "Круг";
            model.setFigure(figure_now);
        }
        private void Triangle_ToolStripButton_Click(object sender, EventArgs e)
        {
            figure_now = "Треугольник";
            model.setFigure(figure_now);
        }
        private void Square_ToolStripButton_Click(object sender, EventArgs e)
        {
            figure_now = "Квадрат";
            model.setFigure(figure_now);
        }
        private void Group_ToolStripButton_Click(object sender, EventArgs e)
        {
            Shape group = new Group();
            for (int i = 0; i < count_cells; ++i)
            {
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())
                    {
                        group.Add_to_Group(ref storage.objects[i]);
                        storage.Delete_object(i);
                    }
            }
            storage.Add_object(count_elements, ref group, count_cells, ref indexin);
        }
        private void Ungrouping_ToolStripButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < count_cells; ++i)
            {
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())
                    {
                        storage.objects[i].Ungroup(ref storage, i);
                        return;
                    }
            }
        }
        bool sticky_now = false;
        private void Stickiness_ToolStripButton_Click(object sender, EventArgs e)
        {
            StickyObserver sticky_observer = new StickyObserver();
            storage.RegisterObserver(sticky_observer);
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())
                    {
                        if (!storage.objects[i].IsSticky() && !sticky_now)
                        {
                            storage.objects[i].Stick(true);
                            sticky_now = true;
                            break;
                        }
                        else
                            if (storage.objects[i].IsSticky() && sticky_now)
                        {
                            storage.objects[i].Stick(false);
                            sticky_now = false;
                        }
                    }
            Redraw_Shapes(ref storage);
        }
        // создаем каталог для файла
        readonly string path = @"C:\Users\User\OneDrive\Рабочий стол\УНИВЕР\2 КУРС\3 СЕМЕСТР\ООП\Лабораторная работа 8\File.txt";
        private void Write_ToolStripButton_Click(object sender, EventArgs e)
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(storage.Occupied(count_cells));
                for (int i = 0; i < count_cells; ++i)
                {
                    if (!storage.Is_empty(i))
                    {
                        sw.WriteLine(storage.objects[i].Save());
                    }
                }
            }
        }
        private void Read_ToolStripButton_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(path, System.Text.Encoding.Default);
            {
                string str = sr.ReadLine();
                int strend = Convert.ToInt32(str);
                for (int i = 0; i < strend; ++i)
                {
                    Shape shape = new Shape();
                    CreateShape create = new CreateShape();
                    create.Choice(ref sr, ref shape, create);
                    if (count_elements == count_cells)
                        storage.Increase_Storage(ref count_cells);
                    storage.Add_object(count_elements, ref shape, count_cells, ref indexin);
                    ++count_elements;
                }
                for (int i = 0; i < count_cells; ++i)
                    if (!storage.Is_empty(i))
                        Draw_Shape(ref storage, i);
                sr.Close();
            }
        }
        private void Clear_toolStripButton_Click(object sender, EventArgs e)
        {
            while (storage.Occupied(count_cells) != 0)
            {
                for (int i = 0; i < count_cells; ++i)
                {
                    if (!storage.Is_empty(i))
                    {
                        storage.Delete_object(i);
                    }
                }
            }
            Canvas_Panel.Refresh();//Обновляем панель
        }
        private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Deselect(ref storage);
            Redraw_Shapes(ref storage);
            int g;
            if (e.Node.Level != 1)
                g = e.Node.Parent.Index;
            else
                g = e.Node.Index;
            storage.objects[g].Select(true);
            Draw_Shape(ref storage, g);
        }
        #endregion
        #region Рисовка и перерисовка фигур
        void Draw_Shape(ref Storage storage, int index)
        {
            Pen pen = new Pen(default_color, 3); // Ручка для рисования
            SolidBrush solidBrush = new SolidBrush(Color.LightGray); // Цвет для заливки
            SolidBrush stickyBrush = new SolidBrush(Color.LightGreen); // Цвет для заливки
            SolidBrush defaultBrush = new SolidBrush(Color.Transparent);
            if (!storage.Is_empty(index))
            {
                pen.Color = storage.objects[index].GetColor();
                if (storage.objects[index].IsSelected() && storage.objects[index].IsSticky())
                    storage.objects[index].Draw_Shape(pen, stickyBrush, Canvas_Panel);
                else if (storage.objects[index].IsSelected() && !storage.objects[index].IsSticky())
                    storage.objects[index].Draw_Shape(pen, solidBrush, Canvas_Panel);
                else
                    storage.objects[index].Draw_Shape(pen, defaultBrush, Canvas_Panel);
            }
        }
        void Redraw_Shapes(ref Storage storage)//Отрисовка объектов 
        {
            Canvas_Panel.Refresh();
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    Draw_Shape(ref storage, i);
        }
        #endregion
        #region Работа с выделением
        void Deselect(ref Storage storage)//Отменяет выделение у всех объектов
        {
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())
                        storage.objects[i].Select(false);
        }
        private void Remove_Selected(ref Storage storage)
        {   // Удаляет выделенные элементы из хранилища
            for (int i = 0; i < count_cells; ++i)
                if (!storage.Is_empty(i))
                    if (storage.objects[i].IsSelected())
                    {
                        storage.Delete_object(i);
                        storage.Sort(count_cells);
                        count_elements--;
                    }
        }
        #endregion
        void UpdateFromMVC(object sender, EventArgs e)//Изменение выбранной фигуры и подсветка ее кнопки
        {
            Color back_color = Color.Transparent;
            Color selected_back_color = Color.LightPink;
            Circle_ToolStripButton.BackColor = back_color;
            Triangle_ToolStripButton.BackColor = back_color;
            Square_ToolStripButton.BackColor = back_color;
            switch (model.getFigure())
            {
                case "Круг":
                    Circle_ToolStripButton.BackColor = selected_back_color;
                    break;
                case "Треугольник":
                    Triangle_ToolStripButton.BackColor = selected_back_color;
                    break;
                case "Квадрат":
                    Square_ToolStripButton.BackColor = selected_back_color;
                    break;
            }
        }
    }
}