using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IIoT_Lab2_Zaitsev
{
    public partial class Form1 : Form
    {
        private int[] Window = new int[2];
        private int[] Door = new int[2];
        private int[] Moving = new int[2];
        private int[] Operator = new int[2];
        private int[] Siren = new int[2];
        private void ConvertAddress()
        {
            Window[0] = Convert.ToInt32(textBox1.Text.Substring(1, textBox1.Text.IndexOf('.') - 1));
            Window[1] = Convert.ToInt32(textBox1.Text.Substring(textBox1.Text.IndexOf('.') + 1));

            Door[0] = Convert.ToInt32(textBox2.Text.Substring(1, textBox2.Text.IndexOf('.') - 1));
            Door[1] = Convert.ToInt32(textBox2.Text.Substring(textBox2.Text.IndexOf('.') + 1));

            Moving[0] = Convert.ToInt32(textBox3.Text.Substring(1, textBox3.Text.IndexOf('.') - 1));
            Moving[1] = Convert.ToInt32(textBox3.Text.Substring(textBox3.Text.IndexOf('.') + 1));

            Operator[0] = Convert.ToInt32(textBox4.Text.Substring(1, textBox4.Text.IndexOf('.') - 1));
            Operator[1] = Convert.ToInt32(textBox4.Text.Substring(textBox4.Text.IndexOf('.') + 1));

            Siren[0] = Convert.ToInt32(textBox5.Text.Substring(1, textBox5.Text.IndexOf('.') - 1));
            Siren[1] = Convert.ToInt32(textBox5.Text.Substring(textBox5.Text.IndexOf('.') + 1));
        }
        Regex regex = new Regex(@"^[A-Z]\d+\.+\d+$");

        public S7PROSIMLib.S7ProSimClass PS = new S7PROSIMLib.S7ProSimClass();
        public Form1()
        {
            InitializeComponent();
        }

        public bool check()
        {
            bool flags = true;
            if (regex.Matches(textBox1.Text).Count == 0)
            {
                flags = false;
                MessageBox.Show("Input address 'Sensor Window' not a right "+ textBox1.Text);
            }
            if (regex.Matches(textBox2.Text).Count == 0)
            {
                flags = false;
                MessageBox.Show("Input address 'Sensor Door' not a right " + textBox2.Text);
            }
            if (regex.Matches(textBox3.Text).Count == 0)
            {
                flags = false;
                MessageBox.Show("Input address 'Sensor Moving' not a right " + textBox3.Text);
            }
            if (regex.Matches(textBox4.Text).Count == 0)
            {
                flags = false;
                MessageBox.Show("Input address 'Operator' not a right " + textBox4.Text);
            }
            if (regex.Matches(textBox5.Text).Count == 0)
            {
                flags = false;
                MessageBox.Show("Input address 'Siren' not a right " + textBox5.Text);
            }
            return flags;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (check())
            {
                click = true;
                PS.Connect();
                ConvertAddress();
                PS.SetScanMode(S7PROSIMLib.ScanModeConstants.ContinuousScan);
                if (PS.GetState() != "ERROR")
                {
                    label6.Text = "Connected";
                }
                else
                {
                    label6.Text = "Error";
                }
            }
        }
        bool click = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (click == true)
            {
                object fl = false, fl1 = false, fl2 = false;
                PS.ReadFlagValue(103, 2, S7PROSIMLib.PointDataTypeConstants.S7_Bit, ref fl);
                if ((bool)fl)
                {
                    pictureBox1.Image = Properties.Resources.WindowAlert;
                    label1.ForeColor = Color.Red;
                }
                else
                {
                    pictureBox1.Image = Properties.Resources.Window;
                    label1.ForeColor = Color.Green;
                }
                PS.ReadFlagValue(103, 3, S7PROSIMLib.PointDataTypeConstants.S7_Bit, ref fl1);
                if ((bool)fl1)
                {
                    pictureBox2.Image = Properties.Resources.DoorAlert;
                    label2.ForeColor = Color.Red;
                }
                else
                {
                    pictureBox2.Image = Properties.Resources.Door;
                    label2.ForeColor = Color.Green;
                }
                PS.ReadFlagValue(103, 1, S7PROSIMLib.PointDataTypeConstants.S7_Bit, ref fl2);
                if ((bool)fl2)
                {
                    pictureBox3.Image = Properties.Resources.MoveSensorAlert;
                    label3.ForeColor = Color.Red;
                }
                else
                {
                    pictureBox3.Image = Properties.Resources.MoveSensor;
                    label3.ForeColor = Color.Green;
                }
                object value = false;
                PS.ReadOutputPoint(Siren[0], Siren[1], S7PROSIMLib.PointDataTypeConstants.S7_Bit, ref value);
                if ((bool)value)
                {
                    label5.Text = "Alarm";
                    label5.ForeColor = Color.Red;
                }
                else if (!(bool)value)
                {
                    label5.Text = "Good";
                    label5.ForeColor = Color.Green;
                }
                toolStripStatusLabel1.Text = PS.GetState();
                toolStripStatusLabel2.Text = "Input:Window " + textBox1.Text + "," + " Door " + textBox2.Text + "," + " Moving " + textBox3.Text + "," + " Operator " + textBox4.Text + ",";
                toolStripStatusLabel3.Text = " Output:Siren " + textBox5.Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            object value = true;
            PS.WriteInputPoint(Operator[0], Operator[1], ref value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            object value = false;
            PS.WriteInputPoint(Operator[0], Operator[1], ref value);
            PS.WriteInputPoint(Moving[0], Moving[1], ref value);
            PS.WriteInputPoint(Window[0], Window[1], ref value);
            PS.WriteInputPoint(Door[0], Door[1], ref value);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.SelectedItem.ToString();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox2.Text = comboBox2.SelectedItem.ToString();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox3.Text = comboBox3.SelectedItem.ToString();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox4.Text = comboBox4.SelectedItem.ToString();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox5.Text = comboBox5.SelectedItem.ToString();
        }
    }
}
