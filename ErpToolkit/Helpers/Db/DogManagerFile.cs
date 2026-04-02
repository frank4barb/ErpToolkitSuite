
namespace ErpToolkit.Helpers.Db
{
    public static class DogManagerFile
    {
        private static readonly NLog.ILogger _logger;
        static DogManagerFile()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************


        public static void CreateInitFile(DogManager dogMng)
        {
            Console.WriteLine($"Genera File all'avvio");
            DbInstall(dogMng, $"{ErpContext.CurrentDirectory}\\DbInstall.sql");
        }




        public static void DbInstall(DogManager dogMng, string percorsoFile)
        {
            if (dogMng == null) throw new Exception($"DbInstall: dogMng == null.");

            // Opzione 1: Scrivere e sovrascrivere il file
            File.WriteAllText(percorsoFile, "-- DbInstall\n");  Console.WriteLine($"DbInstall su {percorsoFile}");

            // Opzione 2: Aggiungere testo al file esistente
            File.AppendAllText(percorsoFile, "-- init\n");
            Console.WriteLine($"Testo aggiunto a {percorsoFile}");

            // Opzione 3: Usare StreamWriter per maggiore controllo
            using (StreamWriter sw = new StreamWriter(percorsoFile, true)) // true per accodare
            {
                //-----------------------------------------------------------------------
                //creazione tabelle e primary key
                foreach (var tab in dogMng.tables.Values)
                {
                    if (tab == null) throw new Exception($"DbInstall: tab == null.");
                    if (tab.fldIcode == null) throw new Exception($"DbInstall: tab.fldIcode [{tab.tableTpy.FullName}] == null.");
                    //copia valore dei singoli campi
                    foreach (var fld in tab.fields)
                    {
                        if (fld == null) throw new Exception($"DbInstall: fld == null.");
                        //---
                        if (fld.optSID)
                        {
                            ////definisce primary key: imposto campo a NOT NULL
                            //sw.WriteLine($"ALTER TABLE {tab.SqlTableName} ALTER COLUMN {fld.SqlFieldName} char(12) NOT NULL;");
                            //definisce primary key: ALTER TABLE Clienti ADD CONSTRAINT PK_Clienti_IDCliente PRIMARY KEY (IDCliente)
                            sw.WriteLine($"ALTER TABLE {tab.SqlTableName} ADD CONSTRAINT PK_{tab.SqlTableName}_{fld.SqlFieldName} PRIMARY KEY ({fld.SqlFieldName});");
                        }
                        //---
                    }
                }
                //creazione foreing key
                foreach (var tab in dogMng.tables.Values)
                {
                    if (tab == null) throw new Exception($"DbInstall: tab == null.");
                    if (tab.fldIcode == null) throw new Exception($"DbInstall: tab.fldIcode [{tab.tableTpy.FullName}] == null.");
                    //copia valore dei singoli campi
                    foreach (var fld in tab.fields)
                    {
                        if (fld == null) throw new Exception($"DbInstall: fld == null.");
                        //---
                        if (fld.optXREF)
                        {
                            if (fld.optMANDATORY)
                            {
                                ////abilita il valore NULL per le chiavi esterne: ALTER TABLE NomeTabella ALTER COLUMN NomeColonna NULL;
                                //sw.WriteLine($"ALTER TABLE {tab.SqlTableName} ALTER COLUMN {fld.SqlFieldName} char(12) NOT NULL;");
                                //definisce chiave esterna:  ALTER TABLE NomeTabella ADD CONSTRAINT FK_NomeTabella_NomeColonna FOREIGN KEY (NomeColonna) REFERENCES TabellaRiferita(ColonnaRiferita);
                                sw.WriteLine($"ALTER TABLE {tab.SqlTableName} ADD CONSTRAINT FK_{tab.SqlTableName}_{fld.SqlFieldName} FOREIGN KEY ({fld.SqlFieldName}) REFERENCES {fld.XrefObj.table.SqlTableName}({fld.XrefObj.table.fldIcode.SqlFieldName});");
                            }
                            else
                            {
                                //abilita il valore NULL per le chiavi esterne: ALTER TABLE NomeTabella ALTER COLUMN NomeColonna NULL;
                                sw.WriteLine($"ALTER TABLE {tab.SqlTableName} ALTER COLUMN {fld.SqlFieldName} char(12) NULL;");
                                //sostituisce i campi bianchi con NULL per le chiavi esterne: UPDATE NomeTabella SET NomeColonna = NULL WHERE NomeColonna = '';
                                sw.WriteLine($"UPDATE {tab.SqlTableName} SET {fld.SqlFieldName} = NULL WHERE {fld.SqlFieldName} = ' ';");
                                //definisce chiave esterna:  ALTER TABLE NomeTabella ADD CONSTRAINT FK_NomeTabella_NomeColonna FOREIGN KEY (NomeColonna) REFERENCES TabellaRiferita(ColonnaRiferita);
                                sw.WriteLine($"ALTER TABLE {tab.SqlTableName} ADD CONSTRAINT FK_{tab.SqlTableName}_{fld.SqlFieldName} FOREIGN KEY ({fld.SqlFieldName}) REFERENCES {fld.XrefObj.table.SqlTableName}({fld.XrefObj.table.fldIcode.SqlFieldName});");
                            }
                        }
                        //---
                    }
                }
                //-----------------------------------------------------------------------
            }
            File.AppendAllText(percorsoFile, "-- end\n\n");
            Console.WriteLine($"Testo aggiunto a {percorsoFile}");
        }





    }
}
