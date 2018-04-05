
public class Arrays {

	public static void main(String[] args) {
//		foreachExample();
		sortExample();

	}

	private static void sortExample() {
		int arr[] = {4,5,2,7,9,0,1,4,3,2};
		
		for (int i = 0 ; i < 10; i++) {
			for (int j = i+1; j < 10; j++) {
				if (arr[i] > arr[j]) {
					int c =arr[i];
					arr[i] = arr[j];
					arr[j] = c;
				}
			}
		}
		
		for (int element : arr) {
			System.out.println(element);
		}
	}

	private static void foreachExample() {
		int arr[] = {1,2,3,4,5,6,7,8};
		
		for (int element : arr) {
			System.out.println(element);
		}		
	}

}
