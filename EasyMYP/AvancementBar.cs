using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WarhammerOnlineHashBuilder;

namespace EasyMYP
{
    public class ProgressEventArgs : EventArgs
    {
        float value;
        bool finished;

        public float Value { get { return value; } }
        public bool IsFinished { get { return finished; } }

        public ProgressEventArgs(float value, bool finished)
        {
            this.value = value;
            this.finished = finished;
        }
    }

    public partial class AvancementBar : Form
    {
        delegate void del_UpdateEvent(ProgressEventArgs e);
        del_UpdateEvent OnUpdateHashEvent;

        public AvancementBar()
        {
            InitializeComponent();

            OnUpdateHashEvent = TreatHashEvent;
        }

        #region Hash Event Handlers
        public void UpdateHashEventHandler(object sender, HashEventArgs e)
        {
            if (progressBar.InvokeRequired)
            {
                Invoke(OnUpdateHashEvent
                    , new ProgressEventArgs(e.Value, (e.State == HashState.Finished)));
            }
            else
            {
                TreatHashEvent(new ProgressEventArgs(e.Value, (e.State == HashState.Finished)));
            }
        }
        #endregion

        public void UpdateOnEvent(ProgressEventArgs e)
        {
            if (progressBar.InvokeRequired)
            {
                Invoke(OnUpdateHashEvent, e);
            }
            else
            {
                TreatHashEvent(e);
            }
        }

        private void TreatHashEvent(ProgressEventArgs e)
        {
            if (e.IsFinished)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                UpdateProgressBar(e.Value * 100);
            }
        }


        private void UpdateProgressBar(float value)
        {
            progressBar.Value = (int)value;
        }
    }
}