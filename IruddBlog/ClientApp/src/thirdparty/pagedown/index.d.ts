declare global {
    namespace Markdown {
        class Converter {
            makeHtml(input: string) : string;
            hooks : IMarkdownHooks
        }

        class Editor {
            constructor(converter: Converter);
            run();
        }

        interface IMarkdownHooks {
            chain(name: string, callback: ((text: string, rgb: any) => void))
        }
    }
}

export { };