/*
    Copyright(c) Microsoft Open Technologies, Inc. All rights reserved.

    The MIT License(MIT)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files(the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions :

    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using ConnectTheDotsIoT;

namespace TwoPushButtons
{
    public sealed partial class MainPage : Page
    {
        private GpioPinValue pushButtonValue;
        ConnectTheDotsHelper ctdHelper;

        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();

            Unloaded += MainPage_Unloaded;

            InitGPIO();

            List<ConnectTheDotsSensor> sensors = new List<ConnectTheDotsSensor> {
                new ConnectTheDotsSensor("AE48969E-DA04-4BF5-AADF-2F850F81BFA3", "Vote", "1 yes, 2 no"),
            };

            ctdHelper = new ConnectTheDotsHelper(serviceBusNamespace: "your-ns",
                eventHubName: "ehdevices",
                keyName: "D1",
                key: "...=",
                displayName: "THEPi",
                organization: "Whips",
                location: "Kirkland",
                sensorList: sensors);
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                nopin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }
            nopushButton = gpio.OpenPin(NOPB_PIN);
            nopin = gpio.OpenPin(NO_PIN);
            yespushButton = gpio.OpenPin(YESPB_PIN);
            yespin = gpio.OpenPin(YES_PIN);

            // Show an error if the pin wasn't initialized properly
            if (nopin == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO No LED pin.";
                return;
            }
            if (nopushButton == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO No Push Button pin.";
                return;
            }
            if (yespin == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO Yes LED pin.";
                return;
            }
            if (yespushButton == null)
            {
                GpioStatus.Text = "There were problems initializing the GPIO Yes Push Button pin.";
                return;
            }

            nopushButton.SetDriveMode(GpioPinDriveMode.Input);
            nopin.SetDriveMode(GpioPinDriveMode.Output);
            yespushButton.SetDriveMode(GpioPinDriveMode.Input);
            yespin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pins initialized correctly.";
        }

        private void MainPage_Unloaded(object sender, object args)
        {
            // Cleanup
            nopin.Dispose();
            nopushButton.Dispose();
            yespin.Dispose();
            yespushButton.Dispose();
        }

        private void FlipLED()
        {
            ConnectTheDotsSensor sensor = ctdHelper.sensors.Find(item => item.measurename == "Vote");
            pushButtonValue = nopushButton.Read();
            if (pushButtonValue == GpioPinValue.Low)
            {
                LED.Fill = redBrush;
                GpioStatus.Text = "Boo!";
                nopin.Write(GpioPinValue.Low);
                sensor.value = 2;
                ctdHelper.SendSensorData(sensor);
                return;
            }
            else if (pushButtonValue == GpioPinValue.High)
            {
                nopin.Write(GpioPinValue.High);
            }
            pushButtonValue = yespushButton.Read();
            if (pushButtonValue == GpioPinValue.Low)
            {
                LED.Fill = greenBrush;
                GpioStatus.Text = "Yay!";
                yespin.Write(GpioPinValue.Low);
                sensor.value = 1;
                ctdHelper.SendSensorData(sensor);
                return;
            }
            else if (pushButtonValue == GpioPinValue.High)
            {
                LED.Fill = grayBrush;
                GpioStatus.Text = "I'm waiting...";
                yespin.Write(GpioPinValue.High);
            }
        }

       

       private void Timer_Tick(object sender, object e)
        {
            FlipLED();
        }

       
        /// <summary>
        /// 
        /// </summary>
        private const int NO_PIN = 27;
        private const int NOPB_PIN = 5;
        private GpioPin nopin;
        private GpioPin nopushButton;
        private const int YES_PIN = 22;
        private const int YESPB_PIN = 6;
        private GpioPin yespin;
        private GpioPin yespushButton;
        private DispatcherTimer timer;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

    }
}
