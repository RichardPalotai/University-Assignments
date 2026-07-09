import java.awt.*;

public class Fractal {
    private Color color;
    private final int x;
    private final int y;
    private final int x2;
    private final int y2;

    public Fractal(Color color, int x, int y, int x2, int y2) {
        this.color = color;
        this.x = x;
        this.y = y;
        this.x2 = x2;
        this.y2 = y2;
    }

    public Color getColor() {
        return color;
    }
    public int getX() {return x;}
    public int getY() {return y;}
    public int getX2() {return x2;}
    public int getY2() {return y2;}
}
