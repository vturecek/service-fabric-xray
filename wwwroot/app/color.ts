export class Color {

    public constructor(
        public r: number,
        public g: number,
        public b: number
    ) { }
    
    public toRBGAString(a:number): string {
        return 'rgba(' + this.r + ',' + this.g + ',' + this.b + ',' + a + ')';
    }

    public static fromHSV(h: number, s: number, v: number): Color {

        h /= 360;
        s /= 100;
        v /= 100;
        let r, g, b = 0;

        let i = Math.floor(h * 6);
        let f = h * 6 - i;
        let p = v * (1 - s);
        let q = v * (1 - f * s);
        let t = v * (1 - (1 - f) * s);

        switch (i % 6) {
            case 0: r = v, g = t, b = p; break;
            case 1: r = q, g = v, b = p; break;
            case 2: r = p, g = v, b = t; break;
            case 3: r = p, g = q, b = v; break;
            case 4: r = t, g = p, b = v; break;
            case 5: r = v, g = p, b = q; break;
        }

        return new Color(Math.round(r * 255), Math.round(g * 255), Math.round(b * 255));
    }
}