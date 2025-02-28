﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace dotNES.Controllers
{
    class NES001Controller : IController
    {
        private int _data;
        private int _serialData;
        private bool _strobing;

        public bool debug;
        // bit:   	 7     6     5     4     3     2     1     0
        // button:	 A B  Select Start  Up Down  Left 

        private readonly Dictionary<Keys, int> _keyMapping = new Dictionary<Keys, int>
        {
            {Keys.A, 7},
            {Keys.S, 6},
            {Keys.RShiftKey, 5},
            {Keys.Enter, 4},
            {Keys.Up, 3},
            {Keys.Down, 2},
            {Keys.Left, 1},
            {Keys.Right, 0},
            {Keys.Escape,1},
            {Keys.Tab, 1},
            {Keys.Shift, 0},
            {Keys.P, 0},  
        };

        public void Strobe(bool on)
        {
            _serialData = _data;
            _strobing = on;
        }

        public int ReadState()
        {
            int ret = ((_serialData & 0x80) > 0).AsByte();
            if (!_strobing)
            {
                _serialData <<= 1;
                _serialData &= 0xFF;
            }
            return ret;
        }


        public void PressKey(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.P:
                    debug ^= true;
                    break;
                case Keys.Escape:
                    Process.GetCurrentProcess().Kill();
                    break;
                case Keys.Tab:
                    Application.Restart();
                    break;
                default:
                    if (!_keyMapping.ContainsKey(e.KeyCode))
                        return;
                    _data |= 1 << _keyMapping[e.KeyCode];
                    break;
            }
        }

        public void ReleaseKey(KeyEventArgs e)
        {
            if (!_keyMapping.ContainsKey(e.KeyCode))
                return;

            _data &= ~(1 << _keyMapping[e.KeyCode]);
        }
    }
}