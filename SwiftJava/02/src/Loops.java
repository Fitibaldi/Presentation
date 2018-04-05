
public class Loops {

	public static void main(String[] args) {
//		whileLoop();
//		doWhile();
		forLoop();

	}

	private static void forLoop() {
		for (int i = 0; i < 10; i++) {
			System.out.println("Index: " + i);
		}
		
	}

	private static void doWhile() {
		int i = 0;
		do {
			System.out.println("Index: " + i);
			i++;
		} while (i < 10);
		
	}

	private static void whileLoop() {
		int i = 0;
		while (i < 10) {
			System.out.println("Index: " + i);
			i++;
		}
		
	}

}
