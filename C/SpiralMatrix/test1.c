#include <stdio.h>

#define SIZE 8 // Define the size of the matrix

int main() {
    int matrix[SIZE][SIZE] = {0};  // Initialize the matrix with zeros
    int value = 1;  // The first value to fill in the matrix
    int center = SIZE/2;
    // Movement directions ((0,1):right, (1,0):down, (0,-1):left, (-1,0):up)

    // EVEN //
    // int i = center, j = center; // Left:cw, Up:ccw
    // int i = center-1, j = center; // Left:ccw, Down:cw
    // int i = center-1, j = center-1; // Right:cw, Down:ccw
    int i = center, j= center-1; // Right:ccw, Up:cw
    
    // ODD //
    //int i = center, j = center;

    // Clockwise (CW)
    // Right
    // int di[] = {0, 1, 0, -1};
    // int dj[] = {1, 0, -1, 0};

    // Left
    // int di[] = {0, -1, 0, 1};
    // int dj[] = {-1, 0, 1, 0};
    
    // Up
    // int di[] = {-1, 0, 1, 0};
    // int dj[] = {0, 1, 0, -1};

    // Down
    // int di[] = {1, 0, -1, 0};
    // int dj[] = {0, -1, 0, 1};

    // Counterclockwise (CCW)
    // Right
    int di[4] = {0, -1, 0, 1};
    int dj[4] = {1, 0, -1, 0};

    // Left
    // int di[] = {0, 1, 0, -1};
    // int dj[] = {-1, 0, 1, 0};
    
    // Up
    // int di[] = {-1, 0, 1, 0};
    // int dj[] = {0, -1, 0, 1};

    // Down
    // int di[] = {1, 0, -1, 0};
    // int dj[] = {0, 1, 0, -1};

    int direction = 0;

    // Fill the center
    matrix[i][j] = value++;

    // Spiral out
    int dim_count = 2;
    for (int steps = 1; steps < SIZE; steps++) {
        while ((dim_count*dim_count) != (value-1)) {
            for (int s = 0; s < (2*dim_count-1); s++)
            {
                if (s % dim_count == 0 && s != 0 || s == 1 || (dim_count == 2 && s != 0))
                {
                    direction = (direction + 1) % 4;  // Change direction
                    printf("(Direction: %d) ", direction);
                }
                
                i += di[direction];
                j += dj[direction];
                matrix[i][j] = value++;
                printf("%d ", matrix[i][j]);
            }
            printf("\n");
        }
        if (dim_count < SIZE)
        {
            dim_count += 1;
        }
        printf("New dim: %d\n", dim_count);
    }

    printf("\n");

    // Print the matrix
    for (int x = 0; x < SIZE; x++) {
        for (int y = 0; y < SIZE; y++) {
            printf("%3d ", matrix[x][y]);
        }
        printf("\n");
    }

    return 1;
}
