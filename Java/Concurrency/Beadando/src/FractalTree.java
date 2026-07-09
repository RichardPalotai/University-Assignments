import javax.swing.*;
import java.awt.*;
import java.util.concurrent.*;
import java.util.concurrent.atomic.AtomicInteger;

public class FractalTree extends Canvas {
    /* Variables with class-wide visibility */
    private static boolean slowMode;
    private static final int MAX_THREAD_NUMBER = 128;
    private static final AtomicInteger fractalNumber = new AtomicInteger(1023);
    private static final BlockingQueue<Fractal> fractalQueue = new LinkedBlockingQueue<>(1023);
    private static ExecutorService fractalExecutor = Executors.newFixedThreadPool(MAX_THREAD_NUMBER);

    /* Recursive function for calculating all drawcalls for the fractal tree */
    public static void makeFractalTree(int x, int y, int angle, int height) {
        if (slowMode) {
            try {Thread.sleep(100);}
            catch (InterruptedException ie) {ie.printStackTrace();}
        }

        if (height == 0) return;

        int x2 = x + (int)(Math.cos(Math.toRadians(angle)) * height * 8);
        int y2 = y + (int)(Math.sin(Math.toRadians(angle)) * height * 8);

        boolean success = false;
        while (!success) {
            success = fractalQueue.offer(new Fractal(height < 5 ? Color.GREEN : Color.BLACK, x, y, x2, y2));
            if(!success) {
                //System.out.println("Couldnt add fractal to queue");
                try {
                    TimeUnit.MILLISECONDS.sleep(500);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }

        fractalExecutor.submit(() -> makeFractalTree(x2, y2, angle-20, height-1));
        fractalNumber.decrementAndGet();
        synchronized (FractalTree.class) {
            FractalTree.class.notifyAll();
        }
        makeFractalTree(x2, y2, angle+20, height-1);
    }

    /* Code for EDT */
    /* Must only contain swing code (draw things on the screen) */
    /* Must not contain calculations (do not use math and compute libraries here) */
    /* No need to understand swing, a simple endless loop that draws lines is enough */
    @Override
    public void paint(Graphics g) {
        //makeFractalTree(390, 480, -90, 10); // Should not be here!
        super.paint(g);
        while(true) {
            try {
                Fractal f = fractalQueue.poll(1000, TimeUnit.MILLISECONDS);
                if (f == null) {
                    //System.out.println("Fractal queue is empty");
                    break;
                }

                g.setColor(f.getColor());
                g.drawLine(f.getX(), f.getY(), f.getX2(), f.getY2());

            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }
    }

    /* Code for main thread */
    public static void main(String args[]) {

        /* Parse args */
        //slowMode = true;
        slowMode = args.length != 0 && Boolean.parseBoolean(args[0]);

        /* Initialize graphical elements and EDT */
        FractalTree tree = new FractalTree();
        JFrame frame = new JFrame();
        frame.setSize(800,600);
        frame.add(tree);
        frame.setVisible(true);
        fractalExecutor.submit(() -> makeFractalTree(400, 550, -90, 10));
        fractalNumber.decrementAndGet();

        synchronized (FractalTree.class) {
            while (fractalNumber.get() > 0) {
                try {
                    FractalTree.class.wait();
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        }
        fractalExecutor.shutdown();
        /* Log success as last step */
        System.out.println("Main has finished");
    }
}