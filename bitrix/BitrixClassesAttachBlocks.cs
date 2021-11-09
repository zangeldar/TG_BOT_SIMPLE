using System;
using System.Collections.Generic;
using System.Text;

namespace bitrix
{
    public class Attach
    {
        public int Id { get; set; }        
        public List<iBlock> Blocks { get; set; }

        public string Color { get; set; }

        public Attach() { }

        public Attach(int id)
        {
            this.Id = id;
        }

        public Attach(int id, List<iBlock> blocks)
        {
            this.Id = id;
            this.Blocks = blocks;
        }
    }

    public interface iBlock { }

    public class BlockFile : iBlock
    {
        public string Link { get; set; }

        // Поля NAME (название файла), SIZE (размер файла) не являются обязательными.
        public string Name { get; set; }
        // Размер файла необходимо указывать в байтах.
        public int Size { get; set; }

        public BlockFile() { }
        public BlockFile(string link)
        {
            this.Link = link;
        }

        public BlockFile(string link, string name)
        {
            this.Link = link;
            this.Name = name;
        }
    }

    public class BlockUser : iBlock
    {
        public string Name { get; set; }

        // Поля AVATAR (аватар) и LINK (ссылка) не являются обязательными.
        public string Avatar { get; set; }
        public string Link { get; set; }
    }

    public class BlockLink : iBlock
    {
        public string Name { get; set; }
        public string Link { get; set; }

        // Вместо ключа LINK можно использовать и ссылки на сущности:
        public string CHAT_ID { get; set; }
        public string USER_ID { get; set; }

        // Поля WIDTH (ширина) и HEIGHT (высота) не являются обязательными, но рекомендуется их указывать уже сейчас, чтобы правильно отобразить изображение.
        public int Width { get; set; }
        public int Height { get; set; }

        // Поля DESC(описание) и PREVIEW(картинка) не являются обязательными полями.
        public string Desc { get; set; }
        public string Preview { get; set; }
    }

    public class BlockMessage : iBlock
    {
        // В тексте доступны bb-коды: USER, CHAT, SEND, PUT, CALL, BR, B, U, I, S, URL.
        public string Message { get; set; }
    }

    public class BlockDelimiter : iBlock
    {
        public Delimiter Delimiter { get; set; }
    }

    public class BlockGrid : iBlock
    {
        public string Name { get; set; }
        // Для ключа VALUE доступны bb-коды: USER, CHAT, SEND, PUT, CALL, BR, B, U, I, S, URL.
        public string Value { get; set; }
        public Display Display { get; set; }
        public int Width { get; set; }
        // цвет значения;
        public string color { get; set; }

        // значение станет ссылкой.
        public string LINK { get; set; }
        // значение станет ссылкой на чат в веб-мессенджере;
        public string CHAT_ID { get; set; }
        // значение станет ссылкой на пользователя в веб-мессенджере;
        public string USER_ID { get; set; }
    }

    public class BlockImage : iBlock
    {
        public string Link { get; set; }

        // Поля NAME (название) и PREVIEW (картинка-превью) не являются обязательными.
        public string Name { get; set; }
        // Рекомендуется заполнять поле PREVIEW с указанием уменьшенной копии изображения, если поле не заполнено используется LINK.
        public string Preview { get; set; }

        // Поля WIDTH (ширина) и HEIGHT (высота) не являются обязательными, но рекомендуется их указывать уже сейчас, чтобы правильно отобразить изображение.
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public enum Display
    {
        BLOCK,  // Доступен ключ WIDTH для указания ширины блока (в пикселях).
        LINE,   // Доступен ключ WIDTH для указания ширины блока (в пикселях).
        COLUMN  // Доступен ключ WIDTH для указания ширины первой колонки (в пикселях).
    }
    public class Delimiter
    {
        public int Size { get; set; }
        public string Color { get; set; }
    }
}
