namespace AudioPreviewApp
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series5 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title4 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title5 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.Windows.Forms.DataVisualization.Charting.Title title6 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.Chart_Time = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.Chart_Frequency = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ListBox_GreatestFrequency = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.ListBox_Statistics = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Frequency)).BeginInit();
            this.SuspendLayout();
            // 
            // Chart_Time
            // 
            chartArea1.Name = "ChartArea1";
            this.Chart_Time.ChartAreas.Add(chartArea1);
            this.Chart_Time.Dock = System.Windows.Forms.DockStyle.Top;
            legend1.Name = "Legend1";
            this.Chart_Time.Legends.Add(legend1);
            this.Chart_Time.Location = new System.Drawing.Point(0, 0);
            this.Chart_Time.Name = "Chart_Time";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.Name = "Channel 1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.Name = "Channel 2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Start Cursor";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Legend = "Legend1";
            series4.Name = "End Cursor";
            this.Chart_Time.Series.Add(series1);
            this.Chart_Time.Series.Add(series2);
            this.Chart_Time.Series.Add(series3);
            this.Chart_Time.Series.Add(series4);
            this.Chart_Time.Size = new System.Drawing.Size(1025, 324);
            this.Chart_Time.TabIndex = 0;
            this.Chart_Time.Text = "Series1";
            title1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            title1.Name = "Title1";
            title1.Text = "Time Domain";
            title2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            title2.Name = "Title2";
            title2.Text = "Time (milliseconds)";
            title3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            title3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            title3.Name = "Title3";
            title3.Text = "Amplitude";
            this.Chart_Time.Titles.Add(title1);
            this.Chart_Time.Titles.Add(title2);
            this.Chart_Time.Titles.Add(title3);
            // 
            // Chart_Frequency
            // 
            chartArea2.Name = "ChartArea1";
            this.Chart_Frequency.ChartAreas.Add(chartArea2);
            this.Chart_Frequency.Dock = System.Windows.Forms.DockStyle.Top;
            legend2.Name = "Legend1";
            this.Chart_Frequency.Legends.Add(legend2);
            this.Chart_Frequency.Location = new System.Drawing.Point(0, 324);
            this.Chart_Frequency.Name = "Chart_Frequency";
            series5.ChartArea = "ChartArea1";
            series5.Legend = "Legend1";
            series5.Name = "Channel 1";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Channel 2";
            this.Chart_Frequency.Series.Add(series5);
            this.Chart_Frequency.Series.Add(series6);
            this.Chart_Frequency.Size = new System.Drawing.Size(1025, 324);
            this.Chart_Frequency.TabIndex = 1;
            this.Chart_Frequency.Text = "Series1";
            title4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold);
            title4.Name = "Title1";
            title4.Text = "Frequency Domain";
            title5.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            title5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            title5.Name = "Title2";
            title5.Text = "Frequency (Hz)";
            title6.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Left;
            title6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold);
            title6.Name = "Title3";
            title6.Text = "Amplitude";
            this.Chart_Frequency.Titles.Add(title4);
            this.Chart_Frequency.Titles.Add(title5);
            this.Chart_Frequency.Titles.Add(title6);
            // 
            // ListBox_GreatestFrequency
            // 
            this.ListBox_GreatestFrequency.FormattingEnabled = true;
            this.ListBox_GreatestFrequency.Location = new System.Drawing.Point(860, 450);
            this.ListBox_GreatestFrequency.Name = "ListBox_GreatestFrequency";
            this.ListBox_GreatestFrequency.Size = new System.Drawing.Size(157, 186);
            this.ListBox_GreatestFrequency.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(854, 434);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Primary Frequency (Ch1)";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(881, 227);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(115, 33);
            this.button2.TabIndex = 5;
            this.button2.Text = "Save Session";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Click_StopRecording);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(854, 124);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Statistics";
            // 
            // ListBox_Statistics
            // 
            this.ListBox_Statistics.FormattingEnabled = true;
            this.ListBox_Statistics.Location = new System.Drawing.Point(860, 140);
            this.ListBox_Statistics.Name = "ListBox_Statistics";
            this.ListBox_Statistics.Size = new System.Drawing.Size(157, 69);
            this.ListBox_Statistics.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 885);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ListBox_Statistics);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ListBox_GreatestFrequency);
            this.Controls.Add(this.Chart_Frequency);
            this.Controls.Add(this.Chart_Time);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Chart_Frequency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_Time;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart_Frequency;
        private System.Windows.Forms.ListBox ListBox_GreatestFrequency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox ListBox_Statistics;
    }
}

