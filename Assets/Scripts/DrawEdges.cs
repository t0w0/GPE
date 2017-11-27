using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawEdges : MonoBehaviour
{

	public Material mat;
	public List<GameObject> gos = new List<GameObject>();
	private Edge[] edges;

	void OnDrawGizmos () {
		//DrawLine ();
	}

	void DrawLine(GameObject g) {
		Mesh m = g.GetComponent<MeshFilter> ().sharedMesh; 
		foreach (Edge e in g.GetComponent<AddToDrawLines>().edges) {
			//Gizmos.color = Color.black;
			//Gizmos.DrawLine(m.vertices[e.vertexIndex[0]], m.vertices[e.vertexIndex[1]]);

			GL.Begin(GL.LINES);
			mat.SetPass(0);
			GL.Color(new Color(mat.color.r, mat.color.g, mat.color.b, mat.color.a));
			GL.Vertex3(m.vertices[e.vertexIndex[0]].x, m.vertices[e.vertexIndex[0]].y, m.vertices[e.vertexIndex[0]].z);
			GL.Vertex3(m.vertices[e.vertexIndex[1]].x, m.vertices[e.vertexIndex[1]].y, m.vertices[e.vertexIndex[1]].z);
			GL.End();
		}
	}

	void OnPostRender() {
		if (!Input.GetMouseButton(0)){
			foreach (GameObject go in gos) {
				//DrawLine (go);
			}
		}
	}

	/// Builds an array of edges that connect to only one triangle.
	/// In other words, the outline of the mesh    
	public static Edge[] BuildManifoldEdges(Mesh mesh)
	{
		// Build a edge list for all unique edges in the mesh
		Edge[] edges = BuildEdges(mesh.vertexCount, mesh.triangles);

		// We only want edges that connect to a single triangle
		ArrayList culledEdges = new ArrayList();
		foreach (Edge edge in edges)
		{
			if (edge.faceIndex[0] == edge.faceIndex[1])
			{
				culledEdges.Add(edge);
			}
		}

		return culledEdges.ToArray(typeof(Edge)) as Edge[];
	}

	/// Builds an array of unique edges
	/// This requires that your mesh has all vertices welded. However on import, Unity has to split
	/// vertices at uv seams and normal seams. Thus for a mesh with seams in your mesh you
	/// will get two edges adjoining one triangle.
	/// Often this is not a problem but you can fix it by welding vertices 
	/// and passing in the triangle array of the welded vertices.
	public static Edge[] BuildEdges(int vertexCount, int[] triangleArray)
	{
		int maxEdgeCount = triangleArray.Length;
		int[] firstEdge = new int[vertexCount + maxEdgeCount];
		int nextEdge = vertexCount;
		int triangleCount = triangleArray.Length / 3;

		for (int a = 0; a < vertexCount; a++)
			firstEdge[a] = -1;

		// First pass over all triangles. This finds all the edges satisfying the
		// condition that the first vertex index is less than the second vertex index
		// when the direction from the first vertex to the second vertex represents
		// a counterclockwise winding around the triangle to which the edge belongs.
		// For each edge found, the edge index is stored in a linked list of edges
		// belonging to the lower-numbered vertex index i. This allows us to quickly
		// find an edge in the second pass whose higher-numbered vertex index is i.
		Edge[] edgeArray = new Edge[maxEdgeCount];

		int edgeCount = 0;
		for (int a = 0; a < triangleCount; a++)
		{
			int i1 = triangleArray[a * 3 + 2];
			for (int b = 0; b < 3; b++)
			{
				int i2 = triangleArray[a * 3 + b];
				if (i1 < i2)
				{
					Edge newEdge = new Edge();
					newEdge.vertexIndex[0] = i1;
					newEdge.vertexIndex[1] = i2;
					newEdge.faceIndex[0] = a;
					newEdge.faceIndex[1] = a;
					edgeArray[edgeCount] = newEdge;

					int edgeIndex = firstEdge[i1];
					if (edgeIndex == -1)
					{
						firstEdge[i1] = edgeCount;
					}
					else
					{
						while (true)
						{
							int index = firstEdge[nextEdge + edgeIndex];
							if (index == -1)
							{
								firstEdge[nextEdge + edgeIndex] = edgeCount;
								break;
							}

							edgeIndex = index;
						}
					}

					firstEdge[nextEdge + edgeCount] = -1;
					edgeCount++;
				}

				i1 = i2;
			}
		}

		// Second pass over all triangles. This finds all the edges satisfying the
		// condition that the first vertex index is greater than the second vertex index
		// when the direction from the first vertex to the second vertex represents
		// a counterclockwise winding around the triangle to which the edge belongs.
		// For each of these edges, the same edge should have already been found in
		// the first pass for a different triangle. Of course we might have edges with only one triangle
		// in that case we just add the edge here
		// So we search the list of edges
		// for the higher-numbered vertex index for the matching edge and fill in the
		// second triangle index. The maximum number of comparisons in this search for
		// any vertex is the number of edges having that vertex as an endpoint.

		for (int a = 0; a < triangleCount; a++)
		{
			int i1 = triangleArray[a * 3 + 2];
			for (int b = 0; b < 3; b++)
			{
				int i2 = triangleArray[a * 3 + b];
				if (i1 > i2)
				{
					bool foundEdge = false;
					for (int edgeIndex = firstEdge[i2]; edgeIndex != -1; edgeIndex = firstEdge[nextEdge + edgeIndex])
					{
						Edge edge = edgeArray[edgeIndex];
						if ((edge.vertexIndex[1] == i1) && (edge.faceIndex[0] == edge.faceIndex[1]))
						{
							edgeArray[edgeIndex].faceIndex[1] = a;
							foundEdge = true;
							break;
						}
					}

					if (!foundEdge)
					{
						Edge newEdge = new Edge();
						newEdge.vertexIndex[0] = i1;
						newEdge.vertexIndex[1] = i2;
						newEdge.faceIndex[0] = a;
						newEdge.faceIndex[1] = a;
						edgeArray[edgeCount] = newEdge;
						edgeCount++;
					}
				}

				i1 = i2;
			}
		}

		Edge[] compactedEdges = new Edge[edgeCount];
		for (int e = 0; e < edgeCount; e++)
			compactedEdges[e] = edgeArray[e];

		return compactedEdges;
	}
}

public class Edge
{
	// The indiex to each vertex
	public int[] vertexIndex = new int[2];
	// The index into the face.
	// (faceindex[0] == faceindex[1] means the edge connects to only one triangle)
	public int[] faceIndex = new int[2];
}

