using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3


{
    public partial class Form1 : Form
    {

        int x = 0;
        int y = 5;
        static int time_ms = 2000;
        bool moving_x_up = false;
        bool moving_x_down = false;
        bool moving_y_up = false;
        bool moving_y_down = false;
        bool moving_x_override = false;
        bool moving_y_override = false;
        bool[] pending_call_x = new bool[10] { false, false, false, false, false, false, false, false, false, false }; //tablica dla przywolywanych wind
        bool[] pending_call_y = new bool[10] { false, false, false, false, false, false, false, false, false, false }; 
        static int[] tab_1st_up = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        static int[] tab_1st_down = new int[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 }; // po to bylo zeby tab.max()
        static int[] tab_2nd_up = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        static int[] tab_2nd_down = new int[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
        bool[] bool_1st = new bool[10] { false, false, false, false, false, false, false, false, false, false };
        bool[] bool_2nd = new bool[10] { false, false, false, false, false, false, false, false, false, false };

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Refreshing();
        }

        private void Refreshing()
        {
            progressBar1.Value = 10 + 10 * x;
            progressBar2.Value = 10 + 10 * y;
            label14.Text = Convert.ToString(x);
            label19.Text = Convert.ToString(y);
            label18.Text = Convert.ToString(x);
            label15.Text = Convert.ToString(y);
            Movement();
            Show_Calls();
            this.Refresh();
        }
        // X - ZEPSUTE - JEDZIE W GORE NA OSTATNIA MAX ZNANA POZYCJE, Y CHYBA OK (NAPRAWIONE?)
        // NIE WYSWIETLA SIE NAPIS(LUB CALA FUNKCJA) ZEBY WYBRAC PIETRO JEZELI SA KLIKNIETE PIETRA OBOK SIEBIE
        private void Movement()
        {
            if (moving_x_up == true && moving_x_down == false && moving_x_override == false)
                label25.Text = "W GÓRĘ";
            if (moving_x_up == false && moving_x_down == true && moving_x_override == false)
                label25.Text = "W DÓŁ";
            if ((moving_x_up == false && moving_x_down == false) || (moving_x_override == true))
                label25.Text = "STOP";
            if (moving_y_up == true && moving_y_down == false && moving_y_override == false)
                label26.Text = "W GÓRĘ";
            if (moving_y_up == false && moving_y_down == true && moving_y_override == false)
                label26.Text = "W DÓŁ";
            if ((moving_y_up == false && moving_y_down == false) || (moving_y_override == true))
                label26.Text = "STOP";
        }

        private void Show_Calls() 
        {
            for (int i=0; i<10; i++)
            {
                string label_number_x = "label" + (38 - i);
                string label_number_y = "label" + (48 - i);
                string label_number_1 = "label" + (62 - i);
                string label_number_2 = "label" + (63 + i);
                string label_pending = "label" + (73 + i);
                if (bool_1st[i] == true)
                {
                    this.Controls[label_number_x].Visible = true;
                    this.Controls[label_number_1].Visible = true;
                }
                else
                {
                    this.Controls[label_number_x].Visible = false;
                    this.Controls[label_number_1].Visible = false;
                }
                if (bool_2nd[i] == true)
                {
                    this.Controls[label_number_y].Visible = true;
                    this.Controls[label_number_2].Visible = true;
                }
                else
                {
                    this.Controls[label_number_y].Visible = false;
                    this.Controls[label_number_2].Visible = false;
                }
                if (pending_call_x[i] == true || pending_call_y[i] == true)
                    this.Controls[label_pending].Visible = true;
                else
                    this.Controls[label_pending].Visible = false;
            }
        }

        private async void WaitingX()
        {
            moving_x_override = true;
            label27.Visible = true;
            await Task.Delay(time_ms);
            label27.Visible = false;
            moving_x_override = false;
        }

        private async void WaitingY()
        {
            moving_y_override = true;
            label28.Visible = true;
            await Task.Delay(time_ms);
            label28.Visible =false;
            moving_y_override = false;
        }

        private void Delete_RequestX(int floor) // rozdzielic na up i down?
        {
            tab_1st_up.SetValue(-1, floor);
            tab_1st_down.SetValue(10, floor);
        }

        private void Delete_RequestY(int floor) // rozdzielic na up i down?
        {
            tab_2nd_up.SetValue(-1, floor);
            tab_2nd_down.SetValue(10, floor);
        }

        private void button31_Click(object sender, EventArgs e)
        {
            time_ms = Convert.ToInt32(textBox1.Text);
        }

        private void Cancel_RequestsX()
        {
            for (int i=0; i<10;i++)
            {
                bool_1st.SetValue(false, i);
            }
        }

        private void Cancel_RequestsY()
        {
            for (int i = 0; i < 10; i++)
            {
                bool_2nd.SetValue(false, i);
            }
        }

        private void ElevatorX(int floor)
        {
            if ((moving_x_down == true && floor > x) || (moving_x_up == true && floor < x))
                pending_call_x[floor] = true;
            else
            {
                bool_1st[floor] = true;
                if (x > floor)
                    tab_1st_down[floor] = floor;
                // CANCEL REQUEST TUTAJ? I PO ELSE albo warunki z moving up down i tablica pending
                else
                    tab_1st_up[floor] = floor;
            }
        }

        private void ElevatorY(int floor)
        {
            if ((moving_y_down == true && floor > y) || (moving_y_up == true && floor < y))
                pending_call_y[floor] = true;
            else
            {
                bool_2nd[floor] = true;
                if (y > floor)
                    tab_2nd_down[floor] = floor;
                else
                    tab_2nd_up[floor] = floor;
            }
        }

        bool Any_Pending()
        {
            for (int i = 0; i < 10; i++)
            {
                if (pending_call_x[i] == true || pending_call_y[i])
                {
                    return true;
                }
            }
            return false;
        }

        private async void Check_Pending(char which_lift)
        {
            int max_x = 0, max_temp_x = 0;
            int min_x = 9, min_temp_x = 9;
            int below_x = 0, above_x = 0;
            int max_y = 0, max_temp_y = 0;
            int min_y = 9, min_temp_y = 9;
            int below_y = 0, above_y = 0;
            int positionX = x, positionY = y;

            if (Any_Pending() == true)
            {

                for (int i = 0; i < 10; i++)
                {
                    if (pending_call_x[i] == true)
                    {
                        max_temp_x = i;
                        min_temp_x = i;
                        if (max_x < max_temp_x)
                            max_x = max_temp_x;
                        if (min_x > min_temp_x)
                            min_x = min_temp_x;
                        if (which_lift == 'x' && i < positionX)
                            below_x++;
                        else above_x++;
                    }
                }

                for (int i = 0; i < 10; i++)
                {
                    if (pending_call_y[i] == true)
                    {
                        max_temp_y = i;
                        min_temp_y = i;
                        if (max_y < max_temp_y)
                            max_y = max_temp_y;
                        if (min_y > min_temp_y)
                            min_y = min_temp_y;
                        if (which_lift == 'y' && i < positionY)
                            below_y++;
                        else above_y++;
                    }
                }

                if (which_lift == 'x' && ((positionX >= positionY && above_x >= below_x) || (positionX < positionY && below_x < above_x))) // dodany drugi warunek (bez warunkow moving chyba)
                {
                    if (above_x >= below_x)
                    {
                        for (int i = 0; i < max_x - positionX; i++)
                        {
                            moving_x_up = true;
                            await Task.Delay(time_ms);
                            Refreshing();
                            x++;
                            if (bool_1st[x] == true || pending_call_x[i] == true)
                            { WaitingX(); bool_1st[x] = false; Delete_RequestX(x); pending_call_x[i] = false; }
                        }
                        moving_x_up = false;
                    }

                    if (above_x < below_x)
                    {
                        for (int i = 0; i < positionX - min_x; i++)
                        {
                            moving_x_down = true;
                            await Task.Delay(time_ms);
                            Refreshing();
                            x--;
                            if (bool_1st[x] == true || pending_call_x[i] == true)
                            { WaitingX(); bool_1st[x] = false; Delete_RequestX(x); pending_call_x[i] = false; }
                        }
                        moving_x_down = false;
                    }
                    WaitingX();
                  //  Cancel_RequestsX(); //spr czy tutaj to dać
                }

                if (which_lift == 'y' && ((positionY >= positionX && above_y >= below_y) || (positionY < positionX && below_y < above_y)))
                {
                    if (above_y >= below_y)
                    {
                        for (int i = 0; i < max_y - positionY; i++)
                        {
                            moving_y_up = true;
                            await Task.Delay(time_ms);
                            Refreshing();
                            y++;
                            if (bool_2nd[y] == true || pending_call_y[i] == true)
                            { WaitingY(); bool_2nd[y] = false; Delete_RequestY(y); pending_call_y[i] = false; }
                        }
                        moving_y_up = false;
                    }

                    if (above_y < below_y)
                    {
                        for (int i = 0; i < positionY - min_y; i++)
                        {
                            moving_y_down = true;
                            await Task.Delay(time_ms);
                            Refreshing();
                            y--;
                            if (bool_2nd[y] == true || pending_call_y[i] == true)
                            { WaitingY(); bool_2nd[y] = false; Delete_RequestY(y); pending_call_y[i] = false; }
                        }
                        moving_y_down = false;
                    }
                    WaitingY();
                    //  Cancel_RequestsY(); //spr czy tutaj to dać
                }
            }
            pending_call_x[x] = false;
            pending_call_y[y] = false;
        }
        // OGOLNIE POSPRAWDZAC TABLICE REQUESTÓW - NIE DZIAŁA POPRAWNIE USUWANIE LOGÓW
        //DODAC WARUNEK jezeli jedzie w gore i jest po drodze (wciecie do warunku Call()) !!!!!!!!!!!!!!!!
        private void Call(int request)
        {
            int positionX = x;
            int positionY = y;
            bool on_going = false;
            // problem - moze zadna nie przyjechac (zrob lepsze warunki)


            if (Math.Abs(x - request) <= Math.Abs(y - request))
            {
                if ((moving_x_down == false && moving_x_up == false)) //!!!!!!!
                { ElevatorX(request); Call_1st_Lift(request, positionX); on_going = true; }
                else if ((moving_x_down == true && request < positionX) || (moving_x_up == true && request > positionX))
                { ElevatorX(request); on_going = true; }
                else if (moving_y_down == false && moving_y_up == false && on_going == false)
                { ElevatorY(request); Call_2nd_Lift(request, positionY); }
                else { pending_call_x[request] = true; }
            }
            else
            //if (Math.Abs(x - request) > Math.Abs(y - request))
            {
                if ((moving_y_down == false && moving_y_up == false)) //!!!!!!!
                { ElevatorY(request); Call_2nd_Lift(request, positionY); on_going = true; }
                else if ((moving_y_down == true && request < positionY) || (moving_y_up == true && request > positionY))
                { ElevatorY(request); on_going = true; }
                else if (moving_x_down == false && moving_x_up == false && on_going == false)
                { ElevatorX(request); Call_1st_Lift(request, positionX); }
                else { pending_call_y[request] = true; }
            }
            on_going = false;
        }

        private async void Call_1st_Lift(int request, int position) // wlasciwie nie powinno byc unlock lock bo pasazerowie moga chciec wybrac kilka pieter
        {

            if (request >= position) // zmiana z >
            {
             //   Lock_1st_Lift();
                for(int i = 0; i < tab_1st_up.Max() - position; i++) //.OrderBy
                {
                    moving_x_up = true;
                    await Task.Delay(time_ms);
                    Refreshing();
                    x++;
                    if (bool_1st[x] == true)
                    { WaitingX(); bool_1st[x] = false; Delete_RequestX(x); }
                }
                Unlock_1st_Lift();
                moving_x_up = false; //DODAC NA KONIEC SPRAWDZANIE CZY JEST TABLICA PENDING[] Z CALLAMI KTÓRE BYŁY NIEZGODNE Z RUCHEM
            }

            if (request < position)
            {
             //   Lock_1st_Lift();
                for (int i = 0; i < position - tab_1st_down.Min(); i++) // zamiast request  ---> ZROB TU NAJMNIEJSZA Z PRZEDZIAŁU
                {
                    moving_x_down = true;
                    await Task.Delay(time_ms);
                    Refreshing();
                    x--;
                    if (bool_1st[x] == true)
                    { WaitingX(); bool_1st[x] = false; Delete_RequestX(x); }
                }
                Unlock_1st_Lift();
                moving_x_down = false;
            }
            WaitingX();
            Cancel_RequestsX();
            Check_Pending('x'); //!!!!!!!
        }

        private async void Call_2nd_Lift(int request, int position)
        {

            if (request >= position)
            {
             //   Lock_2nd_Lift();
                for (int i = 0; i < tab_2nd_up.Max() - position; i++)
                {
                    moving_y_up = true;
                    await Task.Delay(time_ms);
                    Refreshing();
                    y++;
                    if (bool_2nd[y] == true)
                    { WaitingY(); bool_2nd[y] = false; Delete_RequestY(y); }
                }
             //   Unlock_2nd_Lift();
                
                moving_y_up = false;
            }

            if (request < position)
            {
             //   Lock_2nd_Lift();
                for (int i = 0; i < position - tab_2nd_down.Min(); i++)
                {
                    moving_y_down = true;
                    await Task.Delay(time_ms);
                    Refreshing();
                    y--;
                    if (bool_2nd[y] == true)
                    { WaitingY(); bool_2nd[y] = false; Delete_RequestY(y); }

                }
                Unlock_2nd_Lift();
                moving_y_down = false;
            }
            WaitingY();
            Cancel_RequestsY();
            Check_Pending('y'); //!!!!!!!
        }

        private async void Where_1st_Lift(int request) // dodaj moving up down
        {
            int positionX = x;
            if (moving_x_up == false && moving_x_down == false)
            {
            if (request > positionX)
            {
                //   Lock_1st_Lift();
                for (int i = 0; i < tab_1st_up.Max() - positionX; i++) // tu chyba tez zamiast request
                {
                    moving_x_up = true;
                    await Task.Delay(time_ms);
                    Refreshing();
                    x++;
                    if (bool_1st[x] == true)
                    { WaitingX(); bool_1st[x] = false; Delete_RequestX(x); }
                    // dodatkowo funkcja check dla innych pasazerów
                }
                Unlock_1st_Lift();
                moving_x_up = false;
            }

            if (request < positionX)
            {
                //   Lock_1st_Lift();
                for (int i = 0; i < positionX - tab_1st_down.Min(); i++)
                {
                    moving_x_down = true;
                    await Task.Delay(time_ms);
                    Refreshing();
                    x--;
                    if (bool_1st[x] == true)
                    { WaitingX(); bool_1st[x] = false; Delete_RequestX(x); }

                }
                Unlock_1st_Lift();
                moving_x_down = false;
            }
                WaitingX();
                Cancel_RequestsX();
                Check_Pending('x'); //!!!!!!!
            }
        }

        private async void Where_2nd_Lift(int request)
        {
            int positionY = y;
            if (moving_y_up == false && moving_y_down == false)
            {
                if (request > positionY)
                {
                    //   Lock_2nd_Lift();
                    for (int i = 0; i < tab_2nd_up.Max() - positionY; i++)
                    {
                        moving_y_up = true;
                        await Task.Delay(time_ms);
                        Refreshing();
                        y++;
                        if (bool_2nd[y] == true)
                        { WaitingY(); bool_2nd[y] = false; Delete_RequestY(y); }
                        // dodatkowo funkcja check dla innych pasazerów
                    }
                    Unlock_2nd_Lift(); // tutaj bez chyba ze ktos wywola? spr
                    moving_y_up = false;
                }

                if (request < positionY)
                {
                    //   Lock_2nd_Lift();
                    for (int i = 0; i < positionY - tab_2nd_down.Min(); i++)
                    {
                        moving_y_down = true;
                        await Task.Delay(time_ms);
                        Refreshing();
                        y--;
                        if (bool_2nd[y] == true)
                        { WaitingY(); bool_2nd[y] = false; Delete_RequestY(y); }

                    }
                    Unlock_2nd_Lift();
                    moving_y_down = false;
                }
                WaitingY();
                Cancel_RequestsY();
                Check_Pending('y'); //!!!!!!!
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Call(0);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Call(1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Call(2);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Call(3);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Call(4);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Call(5);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Call(6);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Call(7);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Call(8);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Call(9);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ElevatorX(1);
            Where_1st_Lift(1);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            ElevatorX(2);
            Where_1st_Lift(2);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            ElevatorX(3);
            Where_1st_Lift(3);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ElevatorX(4);
            Where_1st_Lift(4);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ElevatorX(6);
            Where_1st_Lift(6);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ElevatorX(7);
            Where_1st_Lift(7);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ElevatorX(8);
            Where_1st_Lift(8);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            ElevatorX(9);
            Where_1st_Lift(9);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            ElevatorX(5);
            Where_1st_Lift(5);
        }

        private void button20_Click(object sender, EventArgs e) // if moving down pending_call dodanie
        {
            ElevatorX(0);
            Where_1st_Lift(0);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            ElevatorY(0);
            Where_2nd_Lift(0);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            ElevatorY(1);
            Where_2nd_Lift(1);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            ElevatorY(2);
            Where_2nd_Lift(2);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            ElevatorY(3);
            Where_2nd_Lift(3);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            ElevatorY(4);
            Where_2nd_Lift(4);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            ElevatorY(5);
            Where_2nd_Lift(5);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            ElevatorY(6);
            Where_2nd_Lift(6);
        }

        private void button22_Click(object sender, EventArgs e)
        {
            ElevatorY(7);
            Where_2nd_Lift(7);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            ElevatorY(8);
            Where_2nd_Lift(8);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            ElevatorY(9);
            Where_2nd_Lift(9);
        }

        private void button32_Click(object sender, EventArgs e) // reset
        {
            x = 0;
            y = 5;
            time_ms = 2000;
            moving_x_up = false;
            moving_x_down = false;
            moving_y_up = false;
            moving_y_down = false;
            moving_x_override = false;
            moving_y_override = false;
            pending_call_x = new bool[10] { false, false, false, false, false, false, false, false, false, false };
            pending_call_y = new bool[10] { false, false, false, false, false, false, false, false, false, false };
            tab_1st_up = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            tab_1st_down = new int[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            tab_2nd_up = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
            tab_2nd_down = new int[10] { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };
            bool_1st = new bool[10] { false, false, false, false, false, false, false, false, false, false };
            bool_2nd = new bool[10] { false, false, false, false, false, false, false, false, false, false };
            textBox1.Text = "2000";
        }

        private void Unlock_1st_Lift()
        {
            button10.Enabled = true;
            button11.Enabled = true;
            button12.Enabled = true;
            button13.Enabled = true;
            button14.Enabled = true;
            button15.Enabled = true;
            button16.Enabled = true;
            button17.Enabled = true;
            button19.Enabled = true;
            button20.Enabled = true;
        }

        private void Lock_1st_Lift()
        {
            button10.Enabled = false;
            button11.Enabled = false;
            button12.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;
            button19.Enabled = false;
            button20.Enabled = false;
        }

        private void Unlock_2nd_Lift()
        {
            button21.Enabled = true;
            button22.Enabled = true;
            button23.Enabled = true;
            button24.Enabled = true;
            button25.Enabled = true;
            button26.Enabled = true;
            button27.Enabled = true;
            button28.Enabled = true;
            button29.Enabled = true;
            button30.Enabled = true;
        }

        private void Lock_2nd_Lift()
        {
            button21.Enabled = false;
            button22.Enabled = false;
            button23.Enabled = false;
            button24.Enabled = false;
            button25.Enabled = false;
            button26.Enabled = false;
            button27.Enabled = false;
            button28.Enabled = false;
            button29.Enabled = false;
            button30.Enabled = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void progressBar2_Click(object sender, EventArgs e)
        {

        }

        private void label53_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

    }
}