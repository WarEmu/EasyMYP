using System;
using System.Windows.Forms;
using nsHashDictionary;

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
        public void UpdateDictionaryEventHandler(object sender, DictionaryEventArgs e)
        {
            if (progressBar.InvokeRequired)
            {
                Invoke(OnUpdateHashEvent
                    , new ProgressEventArgs(e.Value, (e.State == DictionaryState.Finished)));
            }
            else
            {
                TreatHashEvent(new ProgressEventArgs(e.Value, (e.State == DictionaryState.Finished)));
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
            progressBar.Value = (value > 100) ? 100 : (int)value; //DEBUG: to switch back to simple value
        }
    }
}