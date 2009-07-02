


known: eax and edi
eax = (eax ^ edi) - ((edi << 0x18) ^ (edi >> 8))
==> eax = eax + ((edi << 0x18) ^ (edi >> 8)) ^ edi;

known: eax and edi
edi = (edi ^ edx) - ((edx >> 0x12) ^ (edx << 0xE))
edx = (eax ^ ecx) - ((eax << 4) ^ (eax >> 0x1C))
esi == eax

esi = (((edi ^ ecx) - ((ecx << 0x19) ^ (ecx >> 7))) ^ esi) - ((edi >> 0x10) ^ (edi << 0x10))

ecx = (esi ^ ebx) - ((esi >> 0x15) ^ (esi << 0xB))
esi = (esi ^ edi) - ((edi >> 0x12) ^ (edi << 0xE))