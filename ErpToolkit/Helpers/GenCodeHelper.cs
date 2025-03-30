using ErpToolkit.Helpers.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static ErpToolkit.Helpers.ErpError;


namespace ErpToolkit.Helpers
{
    static class GenCodeHelper
    {

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // Genera codici univochi basati su un contatore con variazione al millisecondo
        // L'ultimo codice generato viene memorizzato per evitare duplicazioni
        private static long[] lastTime = new long[] { /*Icode*/0L, /*GasServerUUID*/0L, /*CLIENT_ID*/0L, /*resConnUID*/0L, /*trCode*/0L, /*free*/0L };
        private static object[] lockLastTime = new object[] { new object(), new object(), new object(), new object(), new object(), new object() };
        // --------------------------------------
        // generate a time-based unique code: synchronized over the lastTime array && with retry option
        // return epoch or -1 if error
        private static long GenUniqueEpoch(int idxCode)
        {
            int i; long epochMillisecondi = -1; int DelayBetweenRetriesMs = 100;
            lock (lockLastTime[idxCode]) {
                for (i = 0; i < 15; i++)
                {
                    DateTimeOffset dataOdierna = DateTimeOffset.UtcNow;
                    epochMillisecondi = dataOdierna.ToUnixTimeMilliseconds();
                    if (lastTime[idxCode] - epochMillisecondi < 0) { lastTime[idxCode] = epochMillisecondi; break; } // save last epoch
                    epochMillisecondi = -1; try { Thread.Sleep(DelayBetweenRetriesMs); } catch (ThreadInterruptedException e) { }  // sleep 100 microsecondi
                }
            }
            if (epochMillisecondi < 0) throw new DatabaseException(ERR_DB_CODEGEN, $"GenUniqueEpoch[{idxCode}]: impossiblie generare epoch last[{lastTime[idxCode]}] new[{epochMillisecondi}]", null);
            return (epochMillisecondi);
        }


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static readonly char[] ICODEVAL = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly long NC = ICODEVAL.Length;

        // -------------------------------------------------------------------------
        // Genera parte variabile dell'ICODE: basato sul contatore: lastTime[ idx = 0 ]
        // l'icode generato va in append sullo StringBuffer icode
        internal static string EpochIcode()
        {
            // genera epoch unico
            long epoch = GenUniqueEpoch(0);    // il contatore dell'icode è l'elemento 0 del vettore dei contatori
            return FillIcode_Old(epoch);
        }
        private static string FillIcode_New(long epoch)
        {
            // genera univoco considerando la somma di secondi e millisecondi
            char[] code = new char[8]; int k = 7; epoch -= 1062367200000L;      /* timeZero: 00:00 01/09/2003 */
            while (k >= 0) { code[k--] = ICODEVAL[(int)(epoch % NC)]; epoch /= NC; }
            return new string(code);
        }
        private static string FillIcode_Old(long epoch)
        {
            // genera ICODE nello stesso modo in cui viene fatto nel DHE
            // prima parte sui secondi e ultimi due carattere indicano i millisecondi
            // ma 36*36=1296 => si perdono 296 codici ogni secondo
            char[] code = new char[8]; int k = 7; epoch -= 1062367200000L;      /* timeZero: 00:00 01/09/2003 */
            long sec = epoch / 1000; long ms = epoch % 1000;
            k = 7; while (k >= 0) { code[k--] = '0'; }  // init
            k = 7;
            while (k >= 6) { code[k--] = ICODEVAL[(int)(ms % NC)]; ms /= NC; }  // ms
            while (k >= 0) { code[k--] = ICODEVAL[(int)(sec % NC)]; sec /= NC; }    // sec
            return new string(code);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // -------------------------------------------------------------------------
        // Genera parte variabile del GasServerUUID:  basato sul contatore: lastTime[ idx = 1 ] 
        // l'icode generato va in append sullo StringBuffer icode
        internal static string EpochServerUUIDCode()
        {
            // genera epoch unico
            long epoch = GenUniqueEpoch(1);    // il contatore del clientId è l'elemento 1 del vettore dei contatori
            return FillIcode_Old(epoch);
        }

        // -------------------------------------------------------------------------
        // Genera parte variabile del CLIENT_ID:  basato sul contatore: lastTime[ idx = 2 ] 
        // l'icode generato va in append sullo StringBuffer icode
        internal static string EpochClidCode()
        {
            // genera epoch unico
            long epoch = GenUniqueEpoch(2);    // il contatore del clientId è l'elemento 2 del vettore dei contatori
            return FillIcode_Old(epoch);
        }

        // -------------------------------------------------------------------------
        // Genera parte variabile del resConnUID:  basato sul contatore: lastTime[ idx = 3 ] 
        // l'icode generato va in append sullo StringBuffer icode
        internal static string EpochResConnUIDCode()
        {
            // genera epoch unico
            long epoch = GenUniqueEpoch(3);    // il contatore del resConnUID è l'elemento 3 del vettore dei contatori
            return FillIcode_Old(epoch);
        }

        // -------------------------------------------------------------------------
        // Genera parte variabile del CODICE TRANSAZIONE:  basato sul contatore: lastTime[ idx = 4 ] 
        // l'icode generato va in append sullo StringBuffer icode
        internal static string EpochTransactionCode()
        {
            // genera epoch unico
            long epoch = GenUniqueEpoch(4);    // il contatore del clientId è l'elemento 4 del vettore dei contatori
            return FillIcode_Old(epoch);
        }


        // -------------------------------------------------------------------------
        // Genera timestamp byte[8] da long eg: epoch = System.currentTimeMillis(); (basato sul millisecondo, non necessariamente univoco) 
        internal static byte[] GenLongToTimestamp(long epoch)
        {
            //long epoch = System.currentTimeMillis();
            return (new byte[] {
                (byte) (epoch >> 56),
                (byte) (epoch >> 48),
                (byte) (epoch >> 40),
                (byte) (epoch >> 32),
                (byte) (epoch >> 24),
                (byte) (epoch >> 16),
                (byte) (epoch >> 8),
                (byte) epoch
               });
        }

        // -------------------------------------------------------------------------
        // Genera long da timestamp byte[8] 
        internal static long GenTimestampToLong(byte[] tms)
        {
            long epoch = 0L;
            if (tms != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (i > 0) epoch <<= 8;
                    if (i < tms.Length) epoch += tms[i] & 0xFF;
                }
            }
            return epoch;
        }





    }
}
