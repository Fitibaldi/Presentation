
public class PrintMatrix {

	public static void main(String[] args) {
		int arr[][] = new int[][] { { 1, 2, 3, 4 }, { 5, 6, 7, 8 }, { 9, 10, 11, 12 }, { 13, 14, 15, 16 } };

		// print(arr);
		// print1a(arr);
		print1b(arr);
	}

	private static void print1b(int[][] arr) {	
		int arr2[][] = new int[4][4];
		for (int i = 0; i < arr.length; i++) {
			if (i % 2 == 0) {
				for (int j = 0; j < arr[i].length; j++) {
					arr2[i][j] = arr[i][j];
				}
			} else {
				int k = 0;
				for (int j = arr[i].length - 1; j >= 0; j--, k++) {
					arr2[i][k] = arr[i][j];
				}
			}
		}
		print1a(arr2);
	}

	private static void print1a(int[][] arr) {
		for (int i = 0; i < arr.length; i++) {
			for (int j = 0; j < arr[i].length; j++) {
				System.out.print(arr[j][i] + "\t");
			}
			System.out.println();
		}
	}

	private static void print(int[][] arr) {
		for (int i = 0; i < arr.length; i++) {
			for (int element : arr[i]) {
				System.out.print(element + "\t");
			}
			System.out.println();
		}
	}

}
