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
using System.Media;
using System.Threading;

namespace Fermentor_Alarm
{
    public partial class Fermentor_Alarm : Form
    {
        #region 1. Khai bao
        string sourcePath;
        int count;
        string[] parameters;
        double[] limits;
        SoundPlayer soundAlarm;
        int topMostFlag;
        string lastLines;
        int progressBarFlag;
        #endregion
        #region 2. Main
        public Fermentor_Alarm()
        {
            InitializeComponent();
            timer.Interval = Convert.ToInt32(txtInterval.Text);
            btnStart.Enabled = false;
            btnStart.BackColor = Color.Gray;
            btnStart.ForeColor = Color.DarkGray;
            btnSnooze.Visible = false;
            GetUpdatedLimits();
            soundAlarm = new SoundPlayer();
            soundAlarm.Stream = NAU_Alarm.Properties.Resources.AlarmSound;
            count = 0;
            topMostFlag = 0;
            txtErrorIndicator.Visible = false;
            progressBarFlag = 0;
            LogDown("----- This log was created on " + Convert.ToString(DateTime.Now) + " -----", Convert.ToString(Directory.GetCurrentDirectory()) + "\\NAU_error_log.txt");

            #region 2.1. Enter to setup
            txtTempLow.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        UpdateLimits();
                        CheckLimites();
                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            txtTempUp.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        UpdateLimits();
                        CheckLimites();
                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            txtDOLow.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        UpdateLimits();
                        CheckLimites();
                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            txtDOUp.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        UpdateLimits();
                        CheckLimites();
                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            txtpHLow.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        UpdateLimits();
                        CheckLimites();
                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            txtpHUp.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        UpdateLimits();
                        CheckLimites();

                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            txtInterval.KeyPress += (sndr, ev) =>
            {
                if (ev.KeyChar.Equals((char)13))
                {
                    try
                    {
                        if (Convert.ToInt32(txtInterval.Text) > 100)
                        {
                            timer.Interval = Convert.ToInt32(txtInterval.Text);
                            if (progressBarFlag % 2 == 0)
                            {
                                progressBar.Value = 100;
                            }
                            else
                            {
                                progressBar.Value = 0;
                            }
                            progressBarFlag++;
                        }
                        else
                        {
                            MessageBox.Show("Smallest interval value at 100 ms", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                    }
                    catch (Exception)
                    { return; }
                    ev.Handled = true;
                }
            };
            #endregion
        }
        #endregion 

        #region 3. Limit manipulatate
        private void CheckLimites()
        {
            try
            {
                if ((Convert.ToDouble(parameters[1]) - limits[0]) *
                    (Convert.ToDouble(parameters[1]) - limits[1]) < 0 &
                    (Convert.ToDouble(parameters[2]) - limits[2]) *
                    (Convert.ToDouble(parameters[2]) - limits[3]) < 0 &
                    (Convert.ToDouble(parameters[3]) - limits[4]) *
                    (Convert.ToDouble(parameters[3]) - limits[5]) < 0)
                {
                    count = 0;
                    soundAlarm.Stop();
                    btnSnooze.Visible = false;
                }

                if ((Convert.ToDouble(parameters[1]) - limits[0]) *
                (Convert.ToDouble(parameters[1]) - limits[1]) < 0)
                {
                    labelTemp.ForeColor = Color.OrangeRed;
                    labelTemp.BackColor = Color.FromArgb(30, 30, 30);
                    labelTempValue.ForeColor = Color.OrangeRed;
                    labelTempValue.BackColor = Color.FromArgb(30, 30, 30);
                }
                if ((Convert.ToDouble(parameters[2]) - limits[2]) *
                (Convert.ToDouble(parameters[2]) - limits[3]) < 0)
                {
                    labelDOValue.ForeColor = Color.SpringGreen;
                    labelDOValue.BackColor = Color.FromArgb(30, 30, 30);
                    labelDO.ForeColor = Color.SpringGreen;
                    labelDO.BackColor = Color.FromArgb(30, 30, 30);
                }
                if ((Convert.ToDouble(parameters[3]) - limits[4]) *
                (Convert.ToDouble(parameters[3]) - limits[5]) < 0)
                {
                    labelpHValue.ForeColor = Color.DeepSkyBlue;
                    labelpHValue.BackColor = Color.FromArgb(30, 30, 30);
                    labelpH.ForeColor = Color.DeepSkyBlue;
                    labelpH.BackColor = Color.FromArgb(30, 30, 30);
                }

            }
            catch (Exception)
            {
                return;
            }
        }

        private void UpdateLimits()
        {
            try
            {
                if (Convert.ToDouble(txtTempLow.Text) > limits[1] ||
                    Convert.ToDouble(txtTempUp.Text) < limits[0] ||
                    Convert.ToDouble(txtDOLow.Text) > limits[3] ||
                    Convert.ToDouble(txtDOUp.Text) < limits[2] ||
                    Convert.ToDouble(txtpHLow.Text) > limits[5] ||
                    Convert.ToDouble(txtDOUp.Text) < limits[4])
                {
                    MessageBox.Show("Error when comparing with old values. Please check again!", "Comparision Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                limits[0] = Convert.ToDouble(txtTempLow.Text);
                limits[1] = Convert.ToDouble(txtTempUp.Text);
                limits[2] = Convert.ToDouble(txtDOLow.Text);
                limits[3] = Convert.ToDouble(txtDOUp.Text);
                limits[4] = Convert.ToDouble(txtpHLow.Text);
                limits[5] = Convert.ToDouble(txtpHUp.Text);
                if (progressBarFlag % 2 == 0)
                {
                    progressBar.Value = 100;
                }
                else
                {
                    progressBar.Value = 0;
                }
                progressBarFlag++;

            }
            catch (Exception) 
            { 
                MessageBox.Show("Please check the input of parameters limitation!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
            }
        }

        private void GetUpdatedLimits()
        {
            limits = new double[6] { Convert.ToDouble(txtTempLow.Text),
                Convert.ToDouble(txtTempUp.Text),
                Convert.ToDouble(txtDOLow.Text),
                Convert.ToDouble(txtDOUp.Text),
                Convert.ToDouble(txtpHLow.Text),
                Convert.ToDouble(txtpHUp.Text)};
        }
        #endregion
        #region 4. Keyboard input settings
        private void Number_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            else
            {
                return;
            }
        }

        private void Int_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                return;
            }
        }
        #endregion


        #region 5. Choosing file
        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = "CSV|*.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFileSource.Text = sourcePath = openFileDialog.FileName;
                btnStart.Enabled = true;
                btnStart.ForeColor = Color.DarkGreen;
                btnStart.BackColor = Color.SpringGreen;
            }

        }
        #endregion
        #region 6. Start Button
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                count = 0;
                if (!timer.Enabled)
                {
                    timer.Start();
                    btnStart.Text = "Stop";
                    btnStart.ForeColor = Color.SaddleBrown;
                    btnStart.BackColor = Color.Gold;
                    txtStateSymbol.Text = "⬤";
                    txtStateSymbol.ForeColor = Color.SpringGreen;
                    txtStateText.Text = "Running...";
                    btnChooseFile.Enabled = false;
                    btnChooseFile.ForeColor = Color.DarkGray;
                    btnChooseFile.BackColor = Color.Gray;

                }
                else
                {
                    var result = MessageBox.Show("Are you sure to stop watching?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        timer.Stop();
                        btnSnooze.Visible = false;
                        btnStart.Text = "Start";
                        btnStart.ForeColor = Color.DarkGreen;
                        btnStart.BackColor = Color.SpringGreen;
                        txtStateSymbol.Text = "⬛";
                        txtStateSymbol.ForeColor = Color.OrangeRed;
                        txtStateText.Text = "Stopped";
                        btnChooseFile.Enabled = true;
                        btnChooseFile.BackColor = Color.Gold;
                        btnChooseFile.ForeColor = Color.SaddleBrown;
                        soundAlarm.Stop();
                    }
                    else { return;}
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please check the .csv source file again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion

        #region 7. Timer
        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                count++;
                txtErrorIndicator.Visible = false;
                lastLines= File.ReadAllLines(sourcePath).Reverse().Take(3).Last();
                parameters = lastLines.Split(',');
                txtTime.Text = parameters[0];
                labelTempValue.Text = parameters[1];
                labelDOValue.Text = parameters[2];
                labelpHValue.Text = parameters[3];

                if ((Convert.ToDouble(parameters[1]) - limits[0]) *
                    (Convert.ToDouble(parameters[1]) - limits[1]) > 0 ||
                    (Convert.ToDouble(parameters[2]) - limits[2]) *
                    (Convert.ToDouble(parameters[2]) - limits[3]) > 0 ||
                    (Convert.ToDouble(parameters[3]) - limits[4]) *
                    (Convert.ToDouble(parameters[3]) - limits[5]) > 0) //Nếu thông số nằm ngoài khoảng giới hạn
                {
                    if (count > 0) //Nếu chưa snooze thì báo động
                    {
                        soundAlarm.Play();
                        btnSnooze.Visible = true;
                    }
                    //Set màu cho các thông số
                    if ((Convert.ToDouble(parameters[1]) - limits[0]) *
                    (Convert.ToDouble(parameters[1]) - limits[1]) > 0)
                    {
                        labelTemp.ForeColor = Color.DarkRed;
                        labelTemp.BackColor = Color.OrangeRed;
                        labelTempValue.ForeColor = Color.DarkRed;
                        labelTempValue.BackColor = Color.OrangeRed;
                    }
                    else
                    {
                        labelTemp.ForeColor = Color.OrangeRed;
                        labelTemp.BackColor = Color.FromArgb(30, 30, 30);
                        labelTempValue.ForeColor = Color.OrangeRed;
                        labelTempValue.BackColor = Color.FromArgb(30, 30, 30);
                    }
                    if ((Convert.ToDouble(parameters[2]) - limits[2]) *
                    (Convert.ToDouble(parameters[2]) - limits[3]) > 0)
                    {
                        labelDOValue.ForeColor = Color.DarkGreen;
                        labelDOValue.BackColor = Color.SpringGreen;
                        labelDO.ForeColor = Color.DarkGreen;
                        labelDO.BackColor = Color.SpringGreen;
                    }
                    else
                    {
                        labelDOValue.ForeColor = Color.SpringGreen;
                        labelDOValue.BackColor = Color.FromArgb(30, 30, 30);
                        labelDO.ForeColor = Color.SpringGreen;
                        labelDO.BackColor = Color.FromArgb(30, 30, 30);
                    }
                    if ((Convert.ToDouble(parameters[3]) - limits[4]) *
                    (Convert.ToDouble(parameters[3]) - limits[5]) > 0)
                    {
                        labelpHValue.ForeColor = Color.DarkBlue;
                        labelpHValue.BackColor = Color.DeepSkyBlue;
                        labelpH.ForeColor = Color.DarkBlue;
                        labelpH.BackColor = Color.DeepSkyBlue;
                    }
                    else
                    {
                        labelpHValue.ForeColor = Color.DeepSkyBlue;
                        labelpHValue.BackColor = Color.FromArgb(30, 30, 30);
                        labelpH.ForeColor = Color.DeepSkyBlue;
                        labelpH.BackColor = Color.FromArgb(30, 30, 30);
                    }
                }
                else //Nếu các thông số về lại trong khoảng giới hạn, set lại màu bình thường
                {
                    soundAlarm.Stop();
                    btnSnooze.Visible = false;
                    labelTemp.ForeColor = Color.OrangeRed;
                    labelTemp.BackColor = Color.FromArgb(30, 30, 30);
                    labelTempValue.ForeColor = Color.OrangeRed;
                    labelTempValue.BackColor = Color.FromArgb(30, 30, 30);
                    labelDOValue.ForeColor = Color.SpringGreen;
                    labelDOValue.BackColor = Color.FromArgb(30, 30, 30);
                    labelDO.ForeColor = Color.SpringGreen;
                    labelDO.BackColor = Color.FromArgb(30, 30, 30);
                    labelpHValue.ForeColor = Color.DeepSkyBlue;
                    labelpHValue.BackColor = Color.FromArgb(30, 30, 30);
                    labelpH.ForeColor = Color.DeepSkyBlue;
                    labelpH.BackColor = Color.FromArgb(30, 30, 30);
                }
            }
            catch (Exception)
            {
                LogDown(lastLines, Convert.ToString(Directory.GetCurrentDirectory()) + "\\NAU_error_log.txt");
                txtErrorIndicator.Visible = true;
                return;
            }
        }

        private void LogDown(string _text, string _director)
        {
            using (StreamWriter logFile = new StreamWriter(@_director, true))
            {
                logFile.WriteLine(_text);
            }

        }
        #endregion

        #region 8. Snooze button
        private void btnSnooze_Click(object sender, EventArgs e)
        {
            soundAlarm.Stop();
            count = -4;
            btnSnooze.Visible = false;
        }
        #endregion
        #region 9. Exit confirm
        private void Fermentor_Alarm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CloseCancel() == false)
            {
                e.Cancel = true;
            };
        }

               
        public static bool CloseCancel()
        {
            var result = MessageBox.Show("Wanna close, huh?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
                return true;
            else
                return false;
        }
        #endregion

        #region 10. Always on top setting

        private void checkBoxAlwaysOnTop_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (topMostFlag == 0)
                {
                    Fermentor_Alarm.ActiveForm.TopMost = true;
                    topMostFlag = 1;
                    checkBoxAlwaysOnTop.ForeColor = Color.SpringGreen;
                }
                else
                {
                    Fermentor_Alarm.ActiveForm.TopMost = false;
                    topMostFlag = 0;
                    checkBoxAlwaysOnTop.ForeColor = Color.Gray;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Got an error with Topmost checkbox", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        #endregion
    }

}
