import java.util.Scanner;

public class ConsoleEx {

	public static void main(String[] args) {
		Scanner inputScanner = new Scanner(System.in);
		System.out.print("Please enter name: ");
		String name = inputScanner.next();
		System.out.print("Please enter age: ");
		byte age = inputScanner.nextByte();

		System.out.println("-------------------------");
		System.out.printf("My name is \\\t\"%s\"\nand I am \t%d\n", name, age);
	}

}
