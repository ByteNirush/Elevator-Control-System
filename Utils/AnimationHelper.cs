using System;
using System.Drawing;
using System.Windows.Forms;

namespace ElevatorControlSystem.Utils
{
    /// <summary>
    /// Animation Helper - provides utilities for smooth UI animations
    /// TASK 4: Animation support using Timer and visual effects
    /// </summary>
    public static class AnimationHelper
    {
        /// <summary>
        /// Animate a control's color change
        /// </summary>
        public static void AnimateColorChange(Control control, Color targetColor, int duration = 300)
        {
            try
            {
                if (control.InvokeRequired)
                {
                    control.Invoke(new Action(() => control.BackColor = targetColor));
                }
                else
                {
                    control.BackColor = targetColor;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AnimateColorChange: {ex.Message}");
            }
        }

        /// <summary>
        /// Flash a control to indicate activity
        /// </summary>
        public static void FlashControl(Control control, Color flashColor, int duration = 200)
        {
            try
            {
                Color originalColor = control.BackColor;
                
                System.Windows.Forms.Timer flashTimer = new System.Windows.Forms.Timer();
                flashTimer.Interval = duration;
                flashTimer.Tick += (sender, e) =>
                {
                    flashTimer.Stop();
                    if (control.InvokeRequired)
                    {
                        control.Invoke(new Action(() => control.BackColor = originalColor));
                    }
                    else
                    {
                        control.BackColor = originalColor;
                    }
                    flashTimer.Dispose();
                };

                AnimateColorChange(control, flashColor);
                flashTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FlashControl: {ex.Message}");
            }
        }
    }
}
