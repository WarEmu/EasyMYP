using System;
using System.Collections.Generic;
using System.Text;

namespace WarhammerOnlineHash
{
    public class WarHasher
    {
        uint edx = 0, eax, esi, ebx = 0, ebp;
        uint edi, ecx;

        public uint sh, ph;

        string str_calculating = "";

        public void Hash(string strToHash, uint seed)
        {
            uint string_length = (uint)strToHash.Length;
            uint deadbeef;//= 0xDEADBEEF;
            deadbeef = seed;
            uint A8;
            uint length_left = 0;

            edx = 0; eax = 0; esi = 0; ebx = 0; ebp = 0; edi = 0; ecx = 0;

            eax = string_length;
            esi = string_length + deadbeef;

            ecx = 0;
            edi = esi;
            esi = esi + 0;

            length_left = eax;

            str_calculating = strToHash;

            if (length_left > 0xc)
            {
                eax = length_left - 0xd;
                A8 = (uint)(eax / 12) + 1;
                ebx = edi;

                //L00421900:
                while (A8 > 0)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 7] & 0xff); //*(ebx+7)
                    eax = eax << 8;
                    edx = (uint)((byte)str_calculating[(int)ecx + 6] & 0xff);
                    eax = eax + edx;
                    eax = eax << 8;
                    edx = (uint)((byte)str_calculating[(int)ecx + 5] & 0xff);
                    eax = eax + edx;
                    edx = (uint)((byte)str_calculating[(int)ecx + 4] & 0xff);
                    eax = eax << 8;
                    edx = edx + edi;
                    edi = eax + edx;
                    eax = (uint)((byte)str_calculating[(int)ecx + 0x0B] & 0xff);
                    edx = (uint)((byte)str_calculating[(int)ecx + 0x0A] & 0xff);
                    eax = eax << 8;
                    eax = eax + edx;
                    edx = (uint)((byte)str_calculating[(int)ecx + 9] & 0xff);
                    eax = eax << 8;
                    eax = eax + edx;
                    edx = (uint)((byte)str_calculating[(int)ecx + 8] & 0xff);
                    eax = eax << 8;
                    edx = edx + esi;
                    esi = edx + eax;
                    edx = (uint)((byte)str_calculating[(int)ecx + 3] & 0xff);
                    eax = (uint)((byte)str_calculating[(int)ecx + 2] & 0xff);
                    edx = edx << 8;
                    edx = eax + edx;
                    eax = (uint)((byte)str_calculating[(int)ecx + 1] & 0xff);
                    edx = edx << 8;
                    edx = eax + edx;
                    eax = (uint)((byte)str_calculating[(int)ecx + 0] & 0xff);
                    edx = edx << 8;
                    edx = edx + eax;
                    edx = edx - esi;
                    eax = esi;
                    eax = eax >> 0x1C;
                    edx = edx + ebx;
                    edx = edx ^ eax;
                    eax = esi;
                    eax = eax << 4;
                    edx = edx ^ eax;
                    esi = esi + edi;
                    edi = edi - edx;
                    eax = edx;
                    eax = eax >> 0x1A;
                    edi = edi ^ eax;
                    eax = edx;
                    eax = eax << 6;
                    edi = edi ^ eax;
                    edx = edx + esi;
                    esi = esi - edi;
                    eax = edi;
                    eax = eax >> 0x18;
                    esi = esi ^ eax;
                    eax = edi;
                    eax = eax << 8;
                    esi = esi ^ eax;
                    edi = edi + edx;
                    eax = esi;
                    eax = eax >> 0x10;
                    edx = edx - esi;
                    edx = edx ^ eax;
                    eax = esi;
                    eax = eax << 0x10;
                    edx = edx ^ eax;
                    ebx = edx;
                    esi = esi + edi;
                    edi = edi - ebx;
                    eax = ebx;
                    eax = eax << 0x13;
                    edi = edi ^ eax;
                    eax = ebx;
                    eax = eax >> 0xD;
                    edi = edi ^ eax;
                    ebx = ebx + esi;
                    eax = edi;
                    eax = eax >> 0x1C;
                    esi = esi - edi;
                    esi = esi ^ eax;
                    eax = edi;
                    eax = eax << 4;
                    esi = esi ^ eax;
                    edi = edi + ebx;

                    length_left = length_left - 0xC;
                    ecx = ecx + 0xc;
                    A8 = A8 - 1;
                }
                if (length_left > 0xc)
                {
                    TreatEOS();
                }
                else
                {
                    MySwitch(length_left);
                }
            }
            else
            {
                MySwitch(eax);
            }
        }

        public void TreatEOS()
        {
            //save EDI
            ph = edi;
        }

        public void MySwitch(uint value)
        {
            if (value == 0)
            {
                //save ESI
                sh = esi;
                TreatEOS();
            }
            else
            {
                //the following actually converts a string to a number, 
                //can be done way much more simplier using c# method though
                //but for clarity in debugging I will leave this part as it is now
                if (value >= 12)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 0xB]);
                    eax = eax << 0x18;
                    esi = esi + eax;
                }
                if (value >= 11)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 0xA]);
                    eax = eax << 0x10;
                    esi = esi + eax;
                }
                if (value >= 10)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 9]);
                    eax = eax << 8;
                    esi = esi + eax;
                }
                if (value >= 9)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 8]);
                    //eax = eax << 0;
                    esi = esi + eax;
                }
                if (value >= 8)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 7]);
                    eax = eax << 0x18;
                    edi = edi + eax;
                }
                if (value >= 7)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 6]);
                    eax = eax << 0x10;
                    edi = edi + eax;
                }
                if (value >= 6)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 5]);
                    eax = eax << 8;
                    edi = edi + eax;
                }
                if (value >= 5)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 4]);
                    //eax = eax << 0;
                    edi = edi + eax;
                }
                if (value >= 4)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 3]);
                    eax = eax << 0x18;
                    ebx = ebx + eax;
                }
                if (value >= 3)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 2]);
                    eax = eax << 0x10;
                    ebx = ebx + eax;
                }
                if (value >= 2)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 1]);
                    eax = eax << 8;
                    ebx = ebx + eax;
                }
                if (value >= 1)
                {
                    eax = (uint)((byte)str_calculating[(int)ecx + 0]);
                    //eax = eax << 0;
                    ebx = ebx + eax;

                    //some more operation really annoying
                    esi = esi ^ edi; //                     XOR ESI,EDI
                    eax = edi; //                           MOV EAX,EDI
                    eax = eax >> 0x12; //                     SHR EAX,12
                    ecx = edi;//                            MOV ECX,EDI
                    ecx = ecx << 0xE;//                     SHL ECX,0E
                    eax = eax ^ ecx; //                     XOR EAX,ECX
                    esi = esi - eax;//                      SUB ESI,EAX
                    eax = esi;//                            MOV EAX,ESI
                    eax = eax >> 0x15;//                      SHR EAX,15
                    ecx = esi;//                            MOV ECX,ESI
                    ecx = ecx << 0xB;//                     SHL ECX,0B
                    eax = eax ^ ecx;//                      XOR EAX,ECX
                    ecx = esi;//                            MOV ECX,ESI
                    ecx = ecx ^ ebx;//                      XOR ECX,EBX
                    ecx = ecx - eax;//                      SUB ECX,EAX
                    eax = ecx;//                            MOV EAX,ECX
                    edx = ecx;//                            MOV EDX,ECX
                    edx = edx >> 7;//                       SHR EDX,7
                    edi = edi ^ ecx;//                      XOR EDI,ECX
                    eax = eax << 0x19;//                      SHL EAX,19
                    eax = eax ^ edx;//                      XOR EAX,EDX
                    edi = edi - eax;//                      SUB EDI,EAX
                    esi = edi ^ esi;//                      XOR ESI,EDI
                    eax = edi;//                            MOV EAX,EDI
                    edx = edi;//                            MOV EDX,EDI
                    edx = edx << 0x10;//                      SHL EDX,10
                    eax = eax >> 0x10;//                      SHR EAX,10
                    eax = eax ^ edx;//                      XOR EAX,EDX
                    esi = esi - eax;//                      SUB ESI,EAX
                    eax = esi;//                            MOV EAX,ESI
                    edx = eax;//                            MOV EDX,EAX
                    edx = edx << 4;//                       SHL EDX,4
                    esi = esi >> 0x1C;//                    SHR ESI,1C
                    esi = edx ^ esi;//                      XOR ESI,EDX
                    edx = eax;//                            MOV EDX,EAX
                    edx = edx ^ ecx;//                      XOR EDX,ECX
                    edx = edx - esi;//                      SUB EDX,ESI
                    ecx = edx;//                            MOV ECX,EDX
                    ecx = ecx >> 0x12;//                      SHR ECX,12
                    esi = edx;//                            MOV ESI,EDX
                    esi = esi << 0xE;//                     SHL ESI,0E
                    ecx = ecx ^ esi;//                      XOR ECX,ESI
                    edi = edi ^ edx;//                      XOR EDI,EDX
                    edi = edi - ecx;//                      SUB EDI,ECX
                    ecx = edi;//                            MOV ECX,EDI
                    ecx = ecx << 0x18;//                    SHL ECX,18
                    edx = edi;//                            MOV EDX,EDI
                    edx = edx >> 8;//                       SHR EDX,8
                    ecx = ecx ^ edx;//                      XOR ECX,EDX
                    eax = eax ^ edi;//                      XOR EAX,EDI
                    eax = eax - ecx;//                      SUB EAX,ECX

                    //save EAX
                    sh = eax;

                    //006D12CF  |. 8B4D 10        MOV ECX,DWORD PTR SS:[EBP+10]
                    //006D12D2  |. 8901           MOV DWORD PTR DS:[ECX],EAX
                    TreatEOS();
                }
            }
        }
    }
}
