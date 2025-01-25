#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#define MAX_DIM 20

int allocate_matrix(int matrixExists, int ***current_matrix, int dimension){
    if (matrixExists == 1) // reallocate the memory with the new dimension if matrix was already allocated
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

int load_matrix(int ***current_matrix, int *matrixExists, int *dimension){
    char *filename = NULL;
    int bufferSize = 1; // Initial buffer size
    int index = 0;
    char ch;
    FILE *file = NULL;

    filename = (char *)malloc(bufferSize * sizeof(char)); // Allocate initial memory
    if (filename == NULL) {
        printf("Memory allocation failed\n");
        return 0;
    }

    do
    {
        bufferSize = 1;
        index = 0;
        printf("Filename (e.g.: matrix.txt): ");
        while ((ch = getchar()) != '\n') {
            filename[index++] = ch;
            // If index reaches bufferSize, reallocate more memory
            if (index >= bufferSize) {
                bufferSize *= 2; // Double the buffer size
                char *temp = (char *)realloc(filename, bufferSize * sizeof(char));
                if (temp == NULL) {
                    printf("Memory reallocation failed\n");
                    free(filename);
                    return 0;
                }
                filename = temp;
            }
        }
        filename[index] = '\0'; // Null-terminate the string

        if (strcmp(filename, "quit") == 0)
        {
            free(filename);
            return 0;
        }

        file = fopen(filename, "r");
        if (file == NULL)
        {
            printf("There is no such file\n");
        }
    } while (file == NULL);

    char length_m[MAX_DIM];
    *dimension = 1; // Initialize to 1 to account for the off-by-one issue

    if (fgets(length_m, MAX_DIM, file) != NULL) { // Check that fgets() was successful
        for (int i = 0; i < strlen(length_m); i++) {
            printf("%c", length_m[i]);
            if (length_m[i] == ' ') {
                *dimension += 1;
            }
        }
    } else {
        // Handle error or EOF
        fclose(file);
        free(filename);
        return 0;
    }

    fseek(file, 0, SEEK_SET);

    printf("%d\n", *dimension);
    
    *matrixExists = allocate_matrix(*matrixExists, current_matrix, *dimension);
    if (*matrixExists == 0)
    {
        printf("Memory allocation failed\n");
        fclose(file);
        free(filename);
        return 0;
    }
    
    // Read the file and store the numbers in the matrix
    for(int i = 0; i < *dimension; i++) {
        for(int j = 0; j < *dimension; j++) {
            // Read each number using fscanf
            if (fscanf(file, "%d", &(*current_matrix)[i][j]) != 1) {
                printf("Error reading matrix\n");
                fclose(file);
                free(filename);
                return 0;
            }
        }
    }

    fclose(file);
    free(filename);
    return 1;
}

void display_matrix(int **current_matrix, int dimension){
    for (int i = 0; i < dimension; ++i) {
        for (int j = 0; j < dimension; ++j) {
            printf(" %d ", current_matrix[i][j]);
        }
        printf("\n");
    }
    printf("\n");
}

int main(){
    int dimension = 0;
    int matrixExists = 0;
    int **current_matrix = NULL;
    
    
    if (load_matrix(&current_matrix, &matrixExists, &dimension) == 1)
    {
        printf("\n");
        display_matrix(current_matrix, dimension);
    }
    else
    {
        printf("Failed to load matrix\n");
    }
    
    
    
}