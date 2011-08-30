
namespace nsHasherFunctions
{
    public partial class Hasher
    {

        private void HashSWTOR(string s, uint seed)
        {

            #region MyRegion
            //MOV EAX,DWORD PTR SS:[ESP+C]
            //MOV ECX,DWORD PTR SS:[ESP+4]
            //PUSH EBX
            //PUSH EBP
            //MOV EBP,DWORD PTR SS:[ESP+10]
            //PUSH ESI
            //LEA ESI,DWORD PTR DS:[EAX+EBP+DEADBEEF]
            //PUSH EDI
            //MOV EDI,ESI
            //MOV EBX,ESI
            //CMP EBP,0C
            //JBE swtor.009C5C46 
            #endregion

            eax = ecx = edx = ebx = esi = edi = 0;
            ebx = edi = esi = (uint)s.Length + seed;

            int i = 0;

            for (i = 0; i + 12 < s.Length; i += 12)
            {
                #region MyRegion
                //MOVZX EAX,BYTE PTR DS:[ECX+7]
                //MOVZX EDX,BYTE PTR DS:[ECX+6]
                //SHL EAX,8
                //ADD EAX,EDX
                //MOVZX EDX,BYTE PTR DS:[ECX+5]
                //SHL EAX,8
                //ADD EAX,EDX
                //MOVZX EDX,BYTE PTR DS:[ECX+4]
                //ADD EDX,EDI
                //SHL EAX,8
                //LEA EDI,DWORD PTR DS:[EDX+EAX] 
                #endregion
                edi = (uint)((s[i + 7] << 24) | (s[i + 6] << 16) | (s[i + 5] << 8) | s[i + 4]) + edi; // can be a sum instead of ors if one multiplies correctly
                #region MyRegion
                //MOVZX EAX,BYTE PTR DS:[ECX+B]
                //MOVZX EDX,BYTE PTR DS:[ECX+A]
                //SHL EAX,8
                //ADD EAX,EDX
                //MOVZX EDX,BYTE PTR DS:[ECX+9]
                //SHL EAX,8
                //ADD EAX,EDX
                //MOVZX EDX,BYTE PTR DS:[ECX+8]
                //ADD EDX,ESI
                //SHL EAX,8
                //LEA ESI,DWORD PTR DS:[EDX+EAX] 
                #endregion
                esi = (uint)((s[i + 11] << 24) | (s[i + 10] << 16) | (s[i + 9] << 8) | s[i + 8]) + esi;
                #region MyRegion
                //MOVZX EAX,BYTE PTR DS:[ECX+3]
                //MOVZX EDX,BYTE PTR DS:[ECX+2]
                //SHL EAX,8
                //ADD EAX,EDX
                //MOVZX EDX,BYTE PTR DS:[ECX+1]
                //SHL EAX,8
                //ADD EAX,EDX
                //MOVZX EDX,BYTE PTR DS:[ECX]
                //SHL EAX,8
                //ADD EAX,EDX 
                //SUB EAX,ESI
                #endregion
                eax = (uint)((s[i + 3] << 24) | (s[i + 2] << 16) | (s[i + 1] << 8) | s[i]) - esi; // edx in war, but same anyway

                #region MyRegion
                //eax = eax + ebx;                // ADD EAX,EBX
                //edx = esi;                      // MOV EDX,ESI
                //edx = edx >> 0x1C;              // SHR EDX,1C
                //eax = eax ^ edx;                // XOR EAX,EDX
                //edx = esi;                      // MOV EDX,ESI
                //edx = edx << 4;                 // SHL EDX,4
                //eax = eax ^ edx;                // XOR EAX,EDX 
                #endregion
                eax = ((eax + ebx) ^ (esi >> 0x1C)) ^ (esi << 4);
                esi = esi + edi;                // ADD ESI,EDI
                #region MyRegion
                // SUB EDI,EAX
                // MOV EDX,EAX
                // SHR EDX,1A
                // XOR EDI,EDX
                // MOV EDX,EAX
                // SHL EDX,6
                // XOR EDI,EDX 
                #endregion
                edi = ((edi - eax) ^ (eax >> 0x1A)) ^ (eax << 6);
                eax = eax + esi;                // ADD EAX,ESI
                #region MyRegion
                // SUB ESI,EDI
                // MOV EDX,EDI
                // SHR EDX,18
                // XOR ESI,EDX
                // MOV EDX,EDI
                // SHL EDX,8
                // XOR ESI,EDX 
                #endregion
                esi = ((esi - edi) ^ (edi >> 0x18)) ^ (edi << 8);
                edi = edi + eax;                // ADD EDI,EAX
                #region MyRegion
                // SUB EAX,ESI
                // MOV EDX,ESI
                // SHR EDX,10
                // XOR EAX,EDX
                // MOV EDX,ESI
                // SHL EDX,10
                // XOR EAX,EDX 
                #endregion
                eax = ((eax - esi) ^ (esi >> 0x10)) ^ (esi << 0x10);
                esi = esi + edi;                // ADD ESI,EDI
                #region MyRegion
                // MOV EBX,EAX
                // SUB EDI,EBX
                // SHL EAX,13
                // XOR EDI,EAX
                // MOV EDX,EBX
                // SHR EDX,0D
                // XOR EDI,EDX 
                #endregion
                ebx = eax;
                edi = ((edi - ebx) ^ (ebx << 0x13)) ^ (ebx >> 0x0D);
                ebx = ebx + esi;                // ADD EBX,ESI
                #region MyRegion
                // MOV EAX,EDI
                // SUB ESI,EDI
                // SHR EAX,1C
                // MOV EDX,EDI
                // XOR ESI,EAX
                // SHL EDX,4
                // XOR ESI,EDX 
                #endregion
                esi = ((esi - edi) ^ (edi >> 0x1C)) ^ (edi << 4);
                edi = edi + ebx;// ADD EDI,EBX
            }

            if (s.Length - i > 0)
            {
                switch (s.Length - i)
                {
                    case 12:
                        #region MyRegion
                        //MOVZX EAX,BYTE PTR DS:[ECX+B]
                        //SHL EAX,18
                        //ADD ESI,EAX 
                        #endregion
                        esi += (uint)s[i + 11] << 24;
                        goto case 11;
                    case 11:
                        #region MyRegion
                        //MOVZX EDX,BYTE PTR DS:[ECX+A]
                        //SHL EDX,10
                        //ADD ESI,EDX 
                        #endregion
                        esi += (uint)s[i + 10] << 16;
                        goto case 10;
                    case 10:
                        #region MyRegion
                        //MOVZX EAX,BYTE PTR DS:[ECX+9]
                        //SHL EAX,8
                        //ADD ESI,EAX 
                        #endregion
                        esi += (uint)s[i + 9] << 8;
                        goto case 9;
                    case 9:
                        #region MyRegion
                        //MOVZX EDX,BYTE PTR DS:[ECX+8]
                        //ADD ESI,EDX 
                        #endregion
                        esi += (uint)s[i + 8];
                        goto case 8;
                    case 8:
                        #region MyRegion
                        //MOVZX EAX,BYTE PTR DS:[ECX+7]
                        //SHL EAX,18
                        //ADD EDI,EAX 
                        #endregion
                        edi += (uint)s[i + 7] << 24;
                        goto case 7;
                    case 7:
                        #region MyRegion
                        //MOVZX EDX,BYTE PTR DS:[ECX+6]
                        //SHL EDX,10
                        //ADD EDI,EDX 
                        #endregion
                        edi += (uint)s[i + 6] << 16;
                        goto case 6;
                    case 6:
                        #region MyRegion
                        //MOVZX EAX,BYTE PTR DS:[ECX+5]
                        //SHL EAX,8
                        //ADD EDI,EAX 
                        #endregion
                        edi += (uint)s[i + 5] << 8;
                        goto case 5;
                    case 5:
                        #region MyRegion
                        //MOVZX EDX,BYTE PTR DS:[ECX+4]
                        //ADD EDI,EDX 
                        #endregion
                        edi += (uint)s[i + 4];
                        goto case 4;
                    case 4:
                        #region MyRegion
                        //MOVZX EAX,BYTE PTR DS:[ECX+3]
                        //SHL EAX,18
                        //ADD EBX,EAX 
                        #endregion
                        ebx += (uint)s[i + 3] << 24;
                        goto case 3;
                    case 3:
                        #region MyRegion
                        //MOVZX EDX,BYTE PTR DS:[ECX+2]
                        //SHL EDX,10
                        //ADD EBX,EDX 
                        #endregion
                        ebx += (uint)s[i + 2] << 16;
                        goto case 2;
                    case 2:
                        #region MyRegion
                        //MOVZX EAX,BYTE PTR DS:[ECX+1]
                        //SHL EAX,8
                        //ADD EBX,EAX 
                        #endregion
                        ebx += (uint)s[i + 1] << 8;
                        goto case 1;
                    case 1:
                        #region MyRegion
                        //MOVZX ECX,BYTE PTR DS:[ECX]
                        //ADD EBX,ECX  
                        #endregion
                        ebx += (uint)s[i];
                        break;


                }

                #region MyRegion
                /*
                esi = esi ^ edi;                    // XOR ESI,EDI
                edx = edi;                          // MOV EDX,EDI
                edx = edx >> 0x12;                  // SHR EDX,12
                eax = edi;                          // MOV EAX,EDI
                eax = eax << 0x0E;                  // SHL EAX,0E
                edx = edx ^ eax;                    // XOR EDX,EAX
                esi = esi - edx;                    // SUB ESI,EDX
                */
                #endregion
                esi = (esi ^ edi) - ((edi >> 0x12) ^ (edi << 0x0E));
                #region MyRegion
                /*
                edx = esi;                          // MOV EDX,ESI
                edx = edx >> 0x15;                  // SHR EDX,15
                eax = esi;                          // MOV EAX,ESI
                eax = eax << 0x0B;                  // SHL EAX,0B
                edx = edx ^ eax;                    // XOR EDX,EAX
                ecx = esi;                          // MOV ECX,ESI
                ecx = ecx ^ ebx;                    // XOR ECX,EBX
                ecx = ecx - edx;                    // SUB ECX,EDX
                */

                #endregion
                ecx = (esi ^ ebx) - ((esi >> 0x15) ^ (esi << 0x0B));
                #region MyRegion
                /*
                edx = ecx;                          // MOV EDX,ECX
                edx = edx << 0x19;                  // SHL EDX,19
                eax = ecx;                          // MOV EAX,ECX
                edi = edi ^ ecx;                    // XOR EDI,ECX
                eax = eax >> 0x07;                  // SHR EAX,7
                edx = edx ^ eax;                    // XOR EDX,EAX
                edi = edi - edx;                    // SUB EDI,EDX
                */

                #endregion
                edi = (edi ^ ecx) - ((ecx << 0x19) ^ (ecx >> 0x07));
                #region MyRegion
                /*
                esi = esi ^ edi;                    // XOR ESI,EDI
                edx = edi;                          // MOV EDX,EDI
                edx = edx >> 0x10;                  // SHR EDX,10
                eax = edi;                          // MOV EAX,EDI
                eax = eax << 0x10;                  // SHL EAX,10
                edx = edx ^ eax;                    // XOR EDX,EAX
                esi = esi - edx;                    // SUB ESI,EDX
                */

                #endregion
                esi = (esi ^ edi) - ((edi >> 0x10) ^ (edi << 0x10));
                #region MyRegion
                //eax = esi;                          // MOV EAX,ESI
                //esi = esi >> 0x1C;                  // SHR ESI,1C
                //edx = eax;                          // MOV EDX,EAX
                //edx = edx << 4;                     // SHL EDX,4
                //esi = esi ^ edx;                    // XOR ESI,EDX
                //edx = eax;                          // MOV EDX,EAX
                //edx = edx ^ ecx;                    // XOR EDX,ECX
                //edx = edx - esi;                    // SUB EDX,ESI 
                #endregion
                edx = (esi ^ ecx) - ((esi >> 0x1C) ^ (esi << 0x04));
                #region MyRegion
                //ecx = edx;                          // MOV ECX,EDX
                //esi = edx;                          // MOV ESI,EDX
                //ecx = ecx >> 0x12;                  // SHR ECX,12
                //edi = edi ^ edx;                    // XOR EDI,EDX
                //esi = esi << 0x0E;                  // SHL ESI,0E
                //ecx = ecx ^ esi;                    // XOR ECX,ESI
                //edi = edi - ecx;                    // SUB EDI,ECX 
                #endregion
                edi = (edi ^ edx) - ((edx >> 0x12) ^ (edx << 0x0E));
                #region MyRegion
                //edx = edi;                          // MOV EDX,EDI
                //ecx = edi;                          // MOV ECX,EDI
                //eax = eax ^ edi;                    // XOR EAX,EDI
                ////POP EDI
                //edx = edx << 0x18;                  // SHL EDX,18
                //ecx = ecx >> 0x08;                  // SHR ECX,8
                ////POP ESI
                //edx = edx ^ ecx;                    // XOR EDX,ECX
                ////POP EBP
                //eax = eax - edx;                    // SUB EAX,EDX
                ////POP EBX 
                #endregion
                eax = (esi ^ edi) - ((edi >> 0x08) ^ (edi << 0x18));

                ph = edi;
                sh = eax;

                return;                             //RETN
            }
            ph = esi;
            sh = eax;

            // -------------------------
            // POP EDI
            // MOV EAX,ESI
            // POP ESI
            // POP EBP
            // POP EBX
            return;                 // RETN  


        }
    }
}
