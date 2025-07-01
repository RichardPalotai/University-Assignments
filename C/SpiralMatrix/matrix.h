#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#ifndef matrix
#define matrix


// MEMORY ALLOCATION
int allocate_matrix(int matrixExists, int ***current_matrix, int dimension){
    if (matrixExists == 1) // Reallocate the memory with the new dimension if matrix was already allocated
    {
        *current_matrix = (int **)realloc(*current_matrix, dimension * sizeof(int *));
        if (*current_matrix == NULL)
        {
            printf("MEM_REALLOC_FAILED\n");
            return 0;
        }

        for (int i = 0; i < dimension; ++i)
        {
            (*current_matrix)[i] = (int *)realloc((*current_matrix)[i], dimension * sizeof(int));
            if ((*current_matrix)[i] == NULL)
            {
                printf("MEM_REALLOC_FAILED\n");
                for (int j = 0; j < i; ++j)
                {
                    free((*current_matrix)[j]);
                }
                free(*current_matrix);
                return 0;
            } 
        }
    }
    else // Allocate memory to matrix for the first time
    {
        *current_matrix = (int **)malloc(dimension * sizeof(int *));
        if (*current_matrix == NULL)
        {
            printf("MEM_ALLOC_FAILED\n");
            return 0;
        }

        for (int i = 0; i < dimension; ++i)
        {
            (*current_matrix)[i] = (int *)malloc(dimension * sizeof(int));
            if ((*current_matrix)[i] == NULL)
            {
                printf("MEM_ALLOC_FAILED\n");
                for (int j = 0; j < i; ++j)
                {
                    free((*current_matrix)[j]);
                }
                free(*current_matrix);
                return 0;
            }   
        }
    }
    return 1;
}

// Create the spiral
int make_spiral(int ***current_matrix, char *start_direction, char *rotation_direction, int dimension){
    int value = 1;
    int center = dimension/2;
    int di[4] = {0};
    int dj[4] = {0};
    int i = center, j = center; // ODD //

    if (dimension % 2 == 0) // EVEN //
    {
        if ((start_direction[0] == 'l' && strcmp(rotation_direction, "cw") == 0) || (start_direction[0] == 'u' && strcmp(rotation_direction, "ccw") == 0))
        {
            i = center, j = center; // Left:cw, Up:ccw
        }
        else if ((start_direction[0] == 'l' && strcmp(rotation_direction, "ccw") == 0) || (start_direction[0] == 'd' && strcmp(rotation_direction, "cw") == 0))
        {
            i = center-1, j = center; // Left:ccw, Down:cw
        }
        else if ((start_direction[0] == 'r' && strcmp(rotation_direction, "cw") == 0) || (start_direction[0] == 'd' && strcmp(rotation_direction, "ccw") == 0))
        {
            i = center-1, j = center-1; // Right:cw, Down:ccw
        }
        else if ((start_direction[0] == 'r' && strcmp(rotation_direction, "ccw") == 0) || (start_direction[0] == 'u' && strcmp(rotation_direction, "cw") == 0))
        {
            i = center, j= center-1; // Right:ccw, Up:cw
        }
    }


    // Movement directions ((0,1):right, (1,0):down, (0,-1):left, (-1,0):up)
    if (strcmp(rotation_direction, "cw") == 0) // Clockwise (CW)
    {
        switch (start_direction[0])
        {
        case 'l':
            di[0] = 0;
            di[1] = -1;
            di[2] = 0;
            di[3] = 1;

            dj[0] = -1;
            dj[1] = 0;
            dj[2] = 1;
            dj[3] = 0;
            break;
        case 'r':
            di[0] = 0;
            di[1] = 1;
            di[2] = 0;
            di[3] = -1;

            dj[0] = 1;
            dj[1] = 0;
            dj[2] = -1;
            dj[3] = 0;
            break;
        case 'u':
            di[0] = -1;
            di[1] = 0;
            di[2] = 1;
            di[3] = 0;

            dj[0] = 0;
            dj[1] = 1;
            dj[2] = 0;
            dj[3] = -1;
            break;
        case 'd':
            di[0] = 1;
            di[1] = 0;
            di[2] = -1;
            di[3] = 0;

            dj[0] = 0;
            dj[1] = -1;
            dj[2] = 0;
            dj[3] = 1;
            break;                                
        default:
            break;
        }
    }
    else if (strcmp(rotation_direction, "ccw") == 0) // Counterclockwise (CCW)
    {
        switch (start_direction[0])
        {
        case 'l':
            di[0] = 0;
            di[1] = 1;
            di[2] = 0;
            di[3] = -1;

            dj[0] = -1;
            dj[1] = 0;
            dj[2] = 1;
            dj[3] = 0;
            break;
        case 'r':
            di[0] = 0;
            di[1] = -1;
            di[2] = 0;
            di[3] = 1;

            dj[0] = 1;
            dj[1] = 0;
            dj[2] = -1;
            dj[3] = 0;
            break;
        case 'u':
            di[0] = -1;
            di[1] = 0;
            di[2] = 1;
            di[3] = 0;

            dj[0] = 0;
            dj[1] = -1;
            dj[2] = 0;
            dj[3] = 1;
            break;
        case 'd':
            di[0] = 1;
            di[1] = 0;
            di[2] = -1;
            di[3] = 0;

            dj[0] = 0;
            dj[1] = 1;
            dj[2] = 0;
            dj[3] = -1;
            break;                                
        default:
            break;
        }
    }
    else
    {
        printf("TEST 3\n");
        return 0;
    }

    // Fill the center
    (*current_matrix)[i][j] = value++;

    // Spiral out
    int direction = 0;
    int dim_count = 2;
    for (int steps = 1; steps < dimension; steps++) {
        while ((dim_count*dim_count) != (value-1)) {
            for (int s = 0; s < (2*dim_count-1); s++)
            {
                if (s % dim_count == 0 && s != 0 || s == 1 || (dim_count == 2 && s != 0))
                {
                    direction = (direction + 1) % 4;  // Change direction
                    //printf("(Direction: %d) ", direction);
                }
                
                i += di[direction];
                j += dj[direction];
                (*current_matrix)[i][j] = value++;
                //printf("%d ", *current_matrix[i][j]);
            }
            //printf("\n");
        }
        if (dim_count < dimension)
        {
            dim_count += 1;
        }
        //printf("New dim: %d\n", dim_count);
    }
    //printf("\n");
    return 1;
}

// Display current matrix
void display_matrix(int **current_matrix, int dimension){
    int max_digit = 0;
    int max_num = 0;
    int count = 0;
    for (int i = 0; i < dimension; ++i) {
        for (int j = 0; j < dimension; ++j) {
            max_num = current_matrix[i][j];
            while (max_num > 0)
            {
                count += 1;
                max_num /= 10;
            }
            if (count > max_digit)
            {
                max_digit = count;
            }
            count = 0;
        }
    }

    int num_digit = 0;
    int num = 0;
    char spaces[4];
    for (int i = 0; i < dimension; i++) {
        for (int j = 0; j < dimension; j++) {
            num = current_matrix[i][j];
            while (num > 0)
            {
                num_digit += 1;
                num /= 10;
            }
            for (int s = 0; s < (max_digit-num_digit+1); s++)
            {
                spaces[s] = ' ';
            }
            spaces[max_digit-num_digit+1] = '\0';
            printf("%s%d", spaces, current_matrix[i][j]);
            num_digit = 0;
        }
        printf("\n");
    }
    //printf("Max digit: %d\n", max_digit);
}
#endif