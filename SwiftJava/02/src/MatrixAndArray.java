import java.util.Scanner;
import java.util.concurrent.SynchronousQueue;

public class MatrixAndArray {
	private static final int ARR_SIZE = 5;

	public static void main(String[] args) {
		// simpleArray();
		// matrix();
		averageValue();
	}

	private static void averageValue() {
		Scanner sc = new Scanner(System.in);
		int arr[] = new int[ARR_SIZE];
		int i = 0;
		while (i < ARR_SIZE) {
			System.out.print("Please enter a digit: ");
			arr[i] = sc.nextInt();
			i++;
		}
		
		int sum = 0;
		for (int j = 0; j < arr.length; j++) {
			sum += arr[j];
		}
		
		System.out.println("Average is: " + (sum * 1.0f) / ARR_SIZE);
	}

	private static void matrix() {
		int ma3x[][] = new int[][] { { 5, 6, 7, 8 }, { 1, 2, 3, 4 }, { 8, 8, 7, 9 }, { 7, 8, 5, 1 } };
		for (int i = 0; i < ma3x.length; i++) {
			for (int j = 0; j < ma3x[i].length; j++) {
				System.out.print(ma3x[i][j] + " ");
			}
			System.out.println("");
		}

	}

	private static void simpleArray() {
		int array[] = new int[] { 4, 5, 6, 3, 2 };

		for (int i = 0; i < array.length; i++) {
			System.out.printf("Array[%d] = %d\n", i, array[i]);
		}

	}

}
