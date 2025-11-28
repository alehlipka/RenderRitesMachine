using OpenTK.Mathematics;

namespace RenderRitesMachine.Utilities;

public class Ray(Vector3 origin, Vector3 direction)
{
    public Vector3 Origin { get; } = origin;
    public Vector3 Direction { get; } = direction;

    public Ray TransformToLocalSpace(Matrix4 modelMatrix)
    {
        Matrix4 inverseModel = modelMatrix.Inverted();
        var localOrigin = Vector3.TransformPosition(Origin, inverseModel);
        var localDirection = Vector3.TransformNormal(Direction, inverseModel);
        localDirection.Normalize();
        return new Ray(localOrigin, localDirection);
    }

    public float? IntersectsAABB(Vector3 boxMin, Vector3 boxMax)
    {
        float tMin = float.MinValue;
        float tMax = float.MaxValue;

        for (int i = 0; i < 3; i++)
        {
            if (Math.Abs(Direction[i]) < float.Epsilon)
            {
                if (Origin[i] < boxMin[i] || Origin[i] > boxMax[i])
                {
                    return null;
                }
            }
            else
            {
                float invDir = 1.0f / Direction[i];
                float t1 = (boxMin[i] - Origin[i]) * invDir;
                float t2 = (boxMax[i] - Origin[i]) * invDir;

                if (t1 > t2)
                {
                    (t1, t2) = (t2, t1);
                }

                tMin = Math.Max(tMin, t1);
                tMax = Math.Min(tMax, t2);

                if (tMin > tMax)
                {
                    return null;
                }
            }
        }

        return tMin >= 0 ? tMin : null;
    }

    public static Ray GetFromScreen(float mouseX, float mouseY, Vector2i windowSize, Vector3 cameraPosition,
        Matrix4 projection, Matrix4 view)
    {
        float x = 2.0f * mouseX / windowSize.X - 1.0f;
        float y = 1.0f - 2.0f * mouseY / windowSize.Y;
        Vector3 rayNormalizedDeviceCoords = new(x, y, -1.0f);

        Vector4 rayClip = new(rayNormalizedDeviceCoords.X, rayNormalizedDeviceCoords.Y, -1.0f, 1.0f);
        Vector4 rayEye = rayClip * projection.Inverted();
        rayEye = new Vector4(rayEye.X, rayEye.Y, -1.0f, 0.0f);
        Vector3 rayWorld = (rayEye * view.Inverted()).Xyz;
        rayWorld.Normalize();

        return new Ray(cameraPosition, rayWorld);
    }
}
