
public class Strings {

	public static void main(String[] args) {
		String s1 = "java1sefdsf";
		String s2 = new String ("java2");
		
		System.out.println(s1);
		System.out.println(s2);
		
		// not equal because it is a reference
		System.out.println((s1 == s2) ? "True" : "False");
		
		//
		System.out.println(s1.compareTo(s2));

	}

}
