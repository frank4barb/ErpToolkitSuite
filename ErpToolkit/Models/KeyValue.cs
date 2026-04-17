using System.ComponentModel.DataAnnotations;

namespace ErpToolkit.Models
{
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //!!!!!!!!!!
    //!!!!!!!!!!  SOLUZIONE DA IMPLEMENTARE (attualmente si usa il tipo object)
    //!!!!!!!!!!
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


    // TIPO CHIAVE (ICODE) DELLE TABELLE DB. PUO' ASSUMERE SOLO VALORI: string? o long?
    //
    // USO:

    //KeyValue key = KeyValue.From("IU047HXZLC6R");
    //// oppure
    //KeyValue key = KeyValue.From(123L);


    //switch (key)
    //{
    //    case KeyValue.StringKey s:
    //        // usa s.Value
    //        break;

    //    case KeyValue.LongKey l:
    //        // usa l.Value
    //        break;
    //}


    public abstract record KeyValue
    {
        private KeyValue() { }

        public sealed record StringKey(string Value) : KeyValue;
        public sealed record LongKey(long Value) : KeyValue;

        public static KeyValue From(string value) => new StringKey(value);
        public static KeyValue From(long value) => new LongKey(value);
    }
}
