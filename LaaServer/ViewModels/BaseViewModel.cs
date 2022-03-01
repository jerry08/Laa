using Gress;
using InputSimulatorStandard;
using InputSimulatorStandard.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LaaServer;
using System.Windows.Input;
using LaaServer.Common.Services;
using InTheHand.Net.Sockets;
using InTheHand.Net.Bluetooth;
using Laa.Shared;
using LaaServer.ViewModels.Framework;
using LaaServer.ViewModels.Dialogs;
using Newtonsoft.Json;
using TouchPoint = Laa.Shared.TouchPoint;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;

namespace LaaServer.ViewModels
{
    public class BaseViewModel : PropertyChangedBase
    {
        /*int _mouseScale = 2;
        TouchPoint TouchPointFrom;
        TouchPoint TouchPointTo;
        Task MouseTask;

        private void StartMovingMouse(TouchPoint point)
        {
            decimal steps = 10;

            if (MouseTask != null && !MouseTask.IsCompleted)
            {
                TouchPointTo = point;
                return;
            }

            if (TouchPointFrom == null)
            {
                if (point.TouchActionType != TouchActionType.Released)
                {
                    TouchPointFrom = point;
                }
                
                return;
            }

            TouchPointTo = point;

            MouseTask = Task.Run(async () =>
            {
                //int difX = point.X - TouchPointFrom.X;
                //int difY = point.Y - TouchPointFrom.Y;
                //if (difX > 5 || difX < -5 || difY > 5 || difY < -5)
                //{
                //    while (point.X != TouchPointFrom.X)
                //    {
                //        simulator.Mouse.MoveMouseBy(point.X - TouchPointFrom.X, point.Y - TouchPointFrom.Y);
                //    }
                //    
                //    TouchPointFrom = point;
                //}

                while (TouchPointTo.TouchActionType != TouchActionType.Released)
                {
                    //int x = point.X - TouchPointFrom.X;
                    //int y = point.Y - TouchPointFrom.Y;

                    int x = 0;
                    int y = 0;

                    int diffX = TouchPointTo.X - TouchPointFrom.X;
                    int diffY = TouchPointTo.Y - TouchPointFrom.Y;

                    if (diffX > -10 && diffX < 10)
                    {
                        TouchPointTo.X = TouchPointFrom.X;
                    }

                    if (diffY > -10 && diffY < 10)
                    {
                        TouchPointTo.Y = TouchPointFrom.Y;
                    }

                    decimal x2 = diffX;
                    x = (int)Math.Round(x2 / steps);
                    TouchPointFrom.X += x;

                    decimal y2 = diffY;
                    y = (int)Math.Round(y2 / steps);
                    TouchPointFrom.Y += y;

                    //this.DeviceName = $"x: {x * _mouseScale},  y: {y * _mouseScale}";
                    //this.OnPropertyChanged(null);
                    simulator.Mouse.MoveMouseBy(x * _mouseScale, y * _mouseScale);

                    //Thread.Sleep(2);
                    await Task.Delay(1);
                }

                //TouchPointFrom = TouchPointTo;

                if (TouchPointTo.TouchActionType == TouchActionType.Released)
                {
                    TouchPointFrom = null;
                }
            });
        }

        private void MessageReceived(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            List<string> messages = JsonConvert.DeserializeObject<List<string>>(message);
            for (int i = 0; i < messages.Count; i++)
            {
                PerformAction(messages[i]);
            }
        }

        string prevMessage = "";
        string prevBkMessage = "";
        InputSimulator simulator = new InputSimulator();
        private void PerformAction(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.EndsWith(LaaConstants.MouseLocationHash))
            {
                string val = message.Replace(LaaConstants.MouseLocationHash, "");
                var point = JsonConvert.DeserializeObject<TouchPoint>(val);

                Trace.WriteLine($"x: {point.X}, y: {point.Y}");
                StartMovingMouse(point);
                return;
            }

            if (message.EndsWith(LaaConstants.SecondHash))
            {
                if (prevMessage.EndsWith(LaaConstants.FirstHash))
                {
                    prevMessage = message;
                    return;
                }
            }

            prevMessage = message;

            message = message.Replace(LaaConstants.SecondHash, "").Replace(LaaConstants.FirstHash, "");

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Contains("backspace"))
            {
                if (message.EndsWith(LaaConstants.SecondBkHash))
                {
                    if (prevBkMessage.EndsWith(LaaConstants.FirstBkHash))
                    {
                        prevBkMessage = message;
                        return;
                    }
                }

                prevBkMessage = message;

                int backspaces = message.Split("backspace").Length;
                if (backspaces > 2)
                {
                    simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                }

                simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            }
            else
            {
                simulator.Keyboard.TextEntry(message);
            }
        }*/

        Task MouseTask;
        int _mouseScale;
        /*TouchPoint TouchPointFrom;
        TouchPoint TouchPointTo;

        static void StartMovingMouse(TouchPoint point)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");

            decimal steps = 10;

            if (MouseTask != null && !MouseTask.IsCompleted)
            {
                TouchPointTo = point;
                return;
            }

            if (TouchPointFrom == null)
            {
                if (point.TouchActionType != TouchActionType.Released)
                {
                    TouchPointFrom = point;
                }

                return;
            }

            TouchPointTo = point;

            MouseTask = Task.Run(async () =>
            {
                //int difX = point.X - TouchPointFrom.X;
                //int difY = point.Y - TouchPointFrom.Y;
                //if (difX > 5 || difX < -5 || difY > 5 || difY < -5)
                //{
                //    while (point.X != TouchPointFrom.X)
                //    {
                //        simulator.Mouse.MoveMouseBy(point.X - TouchPointFrom.X, point.Y - TouchPointFrom.Y);
                //    }
                //    
                //    TouchPointFrom = point;
                //}

                int x = 0;
                int y = 0;

                int diffX = TouchPointTo.X - TouchPointFrom.X;
                int diffY = TouchPointTo.Y - TouchPointFrom.Y;

                while (TouchPointTo.TouchActionType != TouchActionType.Released)
                {
                    x = 0;
                    y = 0;
                    diffX = TouchPointTo.X - TouchPointFrom.X;
                    diffY = TouchPointTo.Y - TouchPointFrom.Y;

                    if (diffX > -10 && diffX < 10)
                    {
                        TouchPointTo.X = TouchPointFrom.X;
                    }

                    if (diffY > -10 && diffY < 10)
                    {
                        TouchPointTo.Y = TouchPointFrom.Y;
                    }

                    decimal x2 = diffX;
                    x = (int)Math.Round(x2 / steps);

                    decimal y2 = diffY;
                    y = (int)Math.Round(y2 / steps);

                    if (x * _mouseScale > 10 || x * _mouseScale < -10
                        || y * _mouseScale > 10 || y * _mouseScale < -10)
                    {
                        x = (int)Math.Round(x / 2m);
                        y = (int)Math.Round(y / 2m);
                    }

                    TouchPointFrom.X += x;
                    TouchPointFrom.Y += y;

                    //this.DeviceName = $"x: {x * _mouseScale},  y: {y * _mouseScale}";
                    //this.OnPropertyChanged(null);
                    simulator.Mouse.MoveMouseBy(x * _mouseScale, y * _mouseScale);
                    
                    using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        fs.Seek(0, SeekOrigin.End);
                        byte[] buffer = Encoding.Default.GetBytes($"X: {x * _mouseScale},  Y: {y * _mouseScale},   time: {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffff")}" + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);
                    }

                    //Thread.Sleep(2);
                    await Task.Delay(1);
                }

                //TouchPointFrom = TouchPointTo;

                if (TouchPointTo.TouchActionType == TouchActionType.Released)
                {
                    TouchPointFrom = null;

                    using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        fs.Seek(0, SeekOrigin.End);
                        byte[] buffer = Encoding.Default.GetBytes($"Stopped,   time: {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ffff")}" + " \r\n ");
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            });
        }*/

        /*List<TouchPoint> MainTouchPoints = new List<TouchPoint>();
        void EnqueueMouseMovement(TouchPoint point)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");

            MainTouchPoints.Add(point);

            //var points2 = MainTouchPoints.OrderBy(x => x.DateTimeTicks).ToList();
            //for (int i = 0; i < MainTouchPoints.Count; i++)
            //{
            //    if (points2[i].DateTimeTicks != MainTouchPoints[i].DateTimeTicks)
            //    {
            //
            //    }
            //}

            decimal steps = 10;

            if (MouseTask != null && !MouseTask.IsCompleted)
            {
                return;
            }

            //var task = new Task(async () =>
            MouseTask = Task.Run(() =>
            {
                //await Task.Delay(3000);

                bool end = false;

                while (MainTouchPoints.Count > 1 && !end)
                {
                    //var TouchPoints = MainTouchPoints.ToList().GroupBy(x => new { x.X, x.Y })
                    //    .Select(group => group.FirstOrDefault()).ToList();

                    var TouchPoints = MainTouchPoints.ToList();

                    for (int i = 0; i < TouchPoints.Count; i++)
                    {
                        //Task.Delay(10).Wait();

                        if (i >= TouchPoints.Count - 1)
                        {
                            break;
                        }

                        TouchPoint pointFrom = TouchPoints[i];
                        TouchPoint pointTo = TouchPoints[i + 1];

                        MoveMouse(pointFrom, pointTo);
                        continue;

                        if (pointFrom.X == pointTo.X && pointFrom.Y == pointTo.Y)
                        {
                            continue;
                        }

                        int x = 0;
                        decimal x2 = 0m;

                        int y = 0;
                        decimal y2 = 0m;

                        int diffX = pointTo.X - pointFrom.X;
                        int diffY = pointTo.Y - pointFrom.Y;

                        //x = 0;
                        //y = 0;
                        //diffX = pointTo.X - pointFrom.X;
                        //diffY = pointTo.Y - pointFrom.Y;
                        //
                        //x = diffX;
                        //y = diffY;
                        //
                        //Trace.WriteLine($"x: {pointTo.X}, {x * _mouseScale},  y: {pointTo.Y}, {y * _mouseScale}");
                        //
                        //using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        //{
                        //    fs.Seek(0, SeekOrigin.End);
                        //    byte[] buffer = Encoding.Default.GetBytes($"{x * _mouseScale}, {y * _mouseScale}" + " \r\n ");
                        //    fs.Write(buffer, 0, buffer.Length);
                        //}

                        continue;

                        while (true)
                        {
                            x = 0;
                            y = 0;
                            diffX = pointTo.X - pointFrom.X;
                            diffY = pointTo.Y - pointFrom.Y;

                            if (diffX > -2 && diffX < 2)
                            {
                                pointFrom.X = pointFrom.X;
                            }

                            if (diffY > -2 && diffY < 2)
                            {
                                pointFrom.Y = pointFrom.Y;
                            }

                            x2 = diffX;
                            //x = (int)Math.Ceiling(x2 / steps);
                            x = (int)(MathF.Sign((float)x2 / (float)steps) * MathF.Ceiling(MathF.Abs((float)x2 / (float)steps)));

                            y2 = diffY;
                            //y = (int)Math.Ceiling(y2 / steps);
                            y = (int)(MathF.Sign((float)y2 / (float)steps) * MathF.Ceiling(MathF.Abs((float)y2 / (float)steps)));

                            if (x == 0 && y == 0)
                            {
                                break;
                            }

                            if (x == 0 && y == 0)
                            {
                                break;
                            }

                            if (x * _mouseScale > 6 || x * _mouseScale < -6
                                || y * _mouseScale > 6 || y * _mouseScale < -6)
                            {
                                //x = (int)Math.Ceiling(x / 2m);
                                //y = (int)Math.Ceiling(y / 2m);

                                end = true;
                                break;
                            }

                            pointFrom.X += x;
                            pointFrom.Y += y;

                            //Console.WriteLine($"{x * _mouseScale}, {y * _mouseScale}");
                            Trace.WriteLine($"x: {pointTo.X}, {x * _mouseScale},  y: {pointTo.Y}, {y * _mouseScale}");
                            simulator.Mouse.MoveMouseBy(x * _mouseScale, y * _mouseScale);

                            using (FileStream fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                fs.Seek(0, SeekOrigin.End);
                                byte[] buffer = Encoding.Default.GetBytes($"{x * _mouseScale}, {y * _mouseScale}" + " \r\n ");
                                fs.Write(buffer, 0, buffer.Length);
                            }

                            //Thread.Sleep(2);
                            //await Task.Delay(1);
                            //Task.Delay(1).Wait();
                        }
                    }

                    //TouchPoints.Clear();

                    for (int i = 0; i < TouchPoints.Count; i++)
                    {
                        MainTouchPoints.Remove(TouchPoints[i]);
                    }
                }

                Trace.WriteLine("Done");
            });
        }*/

        List<TouchPoint> MainTouchPoints = new List<TouchPoint>();
        void EnqueueMouseMovement(TouchPoint point)
        {
            //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");

            MainTouchPoints.Add(point);

            //var points2 = MainTouchPoints.OrderBy(x => x.DateTimeTicks).ToList();
            //for (int i = 0; i < MainTouchPoints.Count; i++)
            //{
            //    if (points2[i].DateTimeTicks != MainTouchPoints[i].DateTimeTicks)
            //    {
            //
            //    }
            //}

            if (MouseTask != null && !MouseTask.IsCompleted)
            {
                return;
            }

            //var task = new Task(async () =>
            MouseTask = Task.Run(async () =>
            {
                //await Task.Delay(3000);

                while (MainTouchPoints.Count > 1)
                {
                    //var TouchPoints = MainTouchPoints.ToList().GroupBy(x => new { x.X, x.Y })
                    //    .Select(group => group.FirstOrDefault()).ToList();

                    var TouchPoints = MainTouchPoints.ToList();

                    TouchPoint pointFrom;
                    TouchPoint pointTo;

                    for (int i = 0; i < TouchPoints.Count; i++)
                    {
                        //Task.Delay(10).Wait();

                        if (i < 5 || i % 3 == 0)
                        {
                            await Task.Delay(1);
                        }

                        if (i >= TouchPoints.Count - 1)
                        {
                            break;
                        }

                        pointFrom = TouchPoints[i];
                        pointTo = TouchPoints[i + 1];

                        if (pointFrom.X == pointTo.X && pointFrom.Y == pointTo.Y)
                        {
                            continue;
                        }

                        if ((pointTo.X - pointFrom.X) > 20 || (pointTo.X - pointFrom.X) < -20
                            || (pointTo.Y - pointFrom.Y) > 20 || (pointTo.Y - pointFrom.Y) < -20)
                        {
                            continue;
                        }

                        if ((pointTo.X - pointFrom.X) <= 4 || (pointTo.X - pointFrom.X) >= -4
                            || (pointTo.Y - pointFrom.Y) <= 4 || (pointTo.Y - pointFrom.Y) >= -4)
                        {
                            _mouseScale = 2;
                        }
                        else
                        {
                            _mouseScale = 4;
                        }

                        Trace.WriteLine($"x: {pointTo.X - pointFrom.X}, y: {pointTo.Y - pointFrom.Y}");
                        simulator.Mouse.MoveMouseBy((pointTo.X - pointFrom.X) * _mouseScale, (pointTo.Y - pointFrom.Y) * _mouseScale);
                    }

                    for (int i = 0; i < TouchPoints.Count; i++)
                    {
                        MainTouchPoints.Remove(TouchPoints[i]);
                    }
                }

                Trace.WriteLine("Done");
            });
        }

        void MoveMouse(TouchPoint pointFrom, TouchPoint pointTo)
        {
            if (pointFrom.X == pointTo.X && pointFrom.Y == pointTo.Y)
            {
                return;
            }

            //LinearSmoothMove(new Point(pointFrom.X, pointFrom.Y), new Point(pointTo.X, pointTo.Y), 10);

            int steps = 20;

            int x;
            int y;

            int lastAddedX = 0;
            int lastAddedY = 0;

            int diffX = pointTo.X - pointFrom.X;
            int diffY = pointTo.Y - pointFrom.Y;

            float lastX = 0;
            float lastY = 0;

            float testX = (float)diffX / steps;
            float testY = (float)diffY / steps;

            //while (lastX < diffX && lastY < diffY)
            //while (lastX > diffX && lastY > diffY)
            while (lastX != diffX && lastY != diffY)
            {
                x = 0;
                y = 0;

                //x = (int)(MathF.Sign((float)lastX / (float)steps) * MathF.Ceiling(MathF.Abs((float)lastX / (float)steps)));
                //y = (int)(MathF.Sign((float)y2 / (float)steps) * MathF.Ceiling(MathF.Abs((float)y2 / (float)steps)));

                if (diffX > 0)
                {
                    if (lastX >= lastAddedX)
                    {
                        x = (int)MathF.Ceiling(testX);
                    }
                }
                else if (diffX < 0)
                {
                    if (lastX <= lastAddedX)
                    {
                        x = (int)MathF.Floor(testX);
                    }
                }

                if (diffY > 0)
                {
                    if (lastY >= lastAddedY)
                    {
                        y = (int)MathF.Ceiling(testY);
                    }
                }
                else if (diffY < 0)
                {
                    if (lastY <= lastAddedY)
                    {
                        y = (int)MathF.Floor(testY);
                    }
                }

                lastX += testX;
                lastY += testY;

                lastX = MathF.Round(lastX, 4);
                lastY = MathF.Round(lastY, 4);

                //Convert to positive
                //lastAddedX += x > 0 ? x : -x;
                //lastAddedY += y > 0 ? y : -y;

                lastAddedX += x;
                lastAddedY += y;

                simulator.Mouse.MoveMouseBy(x * _mouseScale, y * _mouseScale);

                //Thread.Sleep(1);
            }
        }

        /*public void LinearSmoothMove(Point newPosition, int steps)
        {
            Point start = GetCursorPosition();
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                SetCursorPosition(Point.Round(iterPoint));
                Thread.Sleep(MouseEventDelayMS);
            }

            // Move the mouse to the final destination.
            SetCursorPosition(newPosition);
        }*/

        /*static int MouseEventDelayMS = 1;
        public static void LinearSmoothMove(Point start, Point newPosition, int steps)
        {
            PointF iterPoint = start;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(newPosition.X - start.X, newPosition.Y - start.Y);

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                //iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                //SetCursorPosition(Point.Round(iterPoint));
                simulator.Mouse.MoveMouseBy(rounded.X * _mouseScale, rounded.Y * _mouseScale);
                Thread.Sleep(MouseEventDelayMS);
            }
        }*/

        string prevMessage = "";
        string prevTapMessage = "";
        string prevBkMessage = "";
        InputSimulator simulator = new InputSimulator();
        public void EnqueueMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.EndsWith(LaaConstants.MouseLocationHash) &&
                !message.Contains(LaaConstants.Tapped1) &&
                !message.Contains(LaaConstants.Tapped2))
            {
                string val = "[" + message.Replace(LaaConstants.MouseLocationHash, "")
                    .Replace("}{", "},{") + "]";

                var points = JsonConvert.DeserializeObject<List<TouchPoint>>(val);
                for (int i = 0; i < points.Count; i++)
                {
                    //StartMovingMouse(points[i]);
                    EnqueueMouseMovement(points[i]);
                }

                return;
            }

            var splitArr = new string[]
            {
                LaaConstants.FirstHash,
                LaaConstants.SecondHash,
                LaaConstants.FirstBkHash,
                LaaConstants.SecondBkHash
            };

            //string[] messages = message.Split(splitArr, StringSplitOptions.None);
            //string[] messages = Regex.Split(message, LaaConstants.FirstHash);
            string[] messages = Regex.Split(message, $@"({LaaConstants.FirstHash}|{LaaConstants.SecondHash}|{LaaConstants.FirstBkHash}|{LaaConstants.SecondBkHash}|{LaaConstants.Tapped1}|{LaaConstants.Tapped2})");

            messages = messages.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i] == LaaConstants.Tapped1 || messages[i] == LaaConstants.Tapped2)
                {
                    PerformAction(messages[i]);
                    continue;
                }

                if (!splitArr.Contains(messages[i]))
                {
                    if (!messages[i].Contains(LaaConstants.MouseLocationHash))
                    {
                        PerformAction(messages[i] + messages[i + 1]);
                    }
                }
            }
        }

        void PerformAction(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Contains(LaaConstants.Tapped2))
            {
                if (prevTapMessage.Contains(LaaConstants.Tapped1))
                {
                    prevTapMessage = message;
                    return;
                }
            }

            if (message.Contains(LaaConstants.Tapped2) || message.Contains(LaaConstants.Tapped1))
            {
                prevTapMessage = message;
            }

            if (message.EndsWith(LaaConstants.SecondHash))
            {
                if (prevMessage.EndsWith(LaaConstants.FirstHash))
                {
                    prevMessage = message;
                    return;
                }
            }

            prevMessage = message;

            message = message.Replace(LaaConstants.SecondHash, "").Replace(LaaConstants.FirstHash, "");

            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (message.Contains("backspace"))
            {
                if (message.EndsWith(LaaConstants.SecondBkHash))
                {
                    if (prevBkMessage.EndsWith(LaaConstants.FirstBkHash))
                    {
                        prevBkMessage = message;
                        return;
                    }
                }

                prevBkMessage = message;

                int backspaces = message.Split("backspace").Length;
                if (backspaces > 2)
                {
                    simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                }

                simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
            }
            else if (message.Contains(LaaConstants.Tapped2) || message.Contains(LaaConstants.Tapped1))
            {
                simulator.Mouse.LeftButtonClick();
            }
            else
            {
                simulator.Keyboard.TextEntry(message);
            }
        }
    }
}