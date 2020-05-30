using GlmNet;

namespace editor
{
    class Camera
    {
        vec3 cOriginPosition = new vec3(0, 0, 10);
        // view port - rendering plane
        vec4 mViewport;
        // camera position
        private vec3 mPosition;
        // eye target location
        private vec3 mTarget;
        // up vector of the camera, for rolling orientation purposes
        private vec3 mUp;
        // taking the camera location, finds the direction and orientation
        // its looking
        private mat4 mLookAt;
        // builds an orthographic projection matrix
        private mat4 mOrtho;
        // world to camera helps to find the world coordinate from a screen pixel coordinate 
        // it its the inverse of the lookat matrix
        private mat4 mWorldToCamera;
        // recalculate the matrices
        private bool mNeedToRecalculate = false;
        // singleton object
        static private Camera sInstance = null;

        static public Camera Instance()
        {
            if (sInstance == null)
            {
                sInstance = new Camera();
            }

            return sInstance;
        }

        private Camera()
        {
            mPosition = cOriginPosition;
            mTarget = new vec3(0, 0, 0);
            mUp = new vec3(0, 1, 0);
            mLookAt = mat4.identity() ;
            mViewport = new vec4(0,0,1,1);
            mOrtho = mat4.identity();
            mWorldToCamera = mat4.identity();
            mNeedToRecalculate = true;
        }

        public void SetToOriginPosition()
        {
            mPosition = cOriginPosition;
            mTarget = new vec3(0, 0, 0);
            mNeedToRecalculate = true;
            // explicitly calls the recalculate method
            //recalculate();
        }

        public void SetCameraPositionX(float xPosition)
        {
            mPosition.x = xPosition;
            mNeedToRecalculate = true;
        }

        public void SetCameraPositionY(float yPosition)
        {
            mPosition.y = yPosition;
            mNeedToRecalculate = true;
        }

        public void SetCameraPositionZ(float zPosition)
        {
            mPosition.z = zPosition;
            mNeedToRecalculate = true;
        }

        public void SetCameraPosition(vec3 position)
        {
            mPosition = position;
            mNeedToRecalculate = true;
        }

        public void MoveCameraOnX(float distance)
        {
            mPosition.x += distance;
            mNeedToRecalculate = true;
        }

        public void MoveCameraOnY(float distance)
        {
            mPosition.y += distance;
            mNeedToRecalculate = true;
        }

        public void MoveCameraOnZ(float distance)
        {
            mPosition.z += distance;
            mNeedToRecalculate = true;
        }

        public void MoveCamera(vec3 distance)
        {
            mPosition += distance;
            mNeedToRecalculate = true;
        }

        internal void SetViewport(int width, int height)
        {
            mViewport = new vec4(0, 0, width, height);
            mNeedToRecalculate = true;
        }

        public void StrifeCameraOnX(float distance)
        {
            mPosition.x += distance;
            mTarget.x += distance;
            mNeedToRecalculate = true;
        }

        public void StrifeCameraOnY(float distance)
        {
            mPosition.y += distance;
            mTarget.y += distance;
            mNeedToRecalculate = true;
        }

        public void StrifeCameraOnZ(float distance)
        {
            mPosition.z += distance;
            mTarget.z += distance;
            mNeedToRecalculate = true;
        }

        public void StrifeCamera(vec3 distance)
        {
            mPosition += distance;
            mTarget += distance;
            mNeedToRecalculate = true;
        }

        public vec3 GetCameraPosition()
        {
            return mPosition;
        }

        public mat4 GetLookAt()
        {
            recalculate();

            return mLookAt;
        }

        public mat4 GetOrtho()
        {
            recalculate();

            return mOrtho;
        }

        public mat4 GetProjectionView()
        {
            recalculate();

            return mOrtho * mLookAt;
        }

        public vec4 GetViewport()
        {
            return mViewport;
        }

        private void recalculate()
        {
            if (mNeedToRecalculate)
            {
                int d = (int)mPosition.z;
                int k = d / 2;
                int width = (int)mViewport.z;
                int height = (int)mViewport.w;

                mOrtho = glm.ortho(
                    -width / 2 + k,
                     width / 2 - k,
                    -height / 2 + k,
                     height / 2 - k,
                     1,
                     100);
                mLookAt = glm.lookAt(mPosition, mTarget, mUp);
                mWorldToCamera = glm.inverse(mLookAt);
                mNeedToRecalculate = false;
            }
        }

        /// <summary>
        /// Find out what is the world location of a point based on current camera parameters
        /// and the current display position
        /// </summary>
        /// <param name="topLeft"> top left display location [input] </param>
        /// <param name="bottomRight"> bottom right display location [input]</param>
        /// <param name="RealTopLeft">top left real world location [out]</param>
        /// <param name="RealBottomRight">bottom right real world location [out]</param>
        internal void WorldToPixel(vec3 topLeft, vec3 bottomRight, ref vec3 RealTopLeft, ref vec3 RealBottomRight)
        {
            float a, b, c, w;

            a = topLeft[0] * mWorldToCamera[0][0] + topLeft[1] * mWorldToCamera[1][0] + topLeft[2] * mWorldToCamera[2][0] + mWorldToCamera[3][0];
            b = topLeft[0] * mWorldToCamera[0][1] + topLeft[1] * mWorldToCamera[1][1] + topLeft[2] * mWorldToCamera[2][1] + mWorldToCamera[3][1];
            c = topLeft[0] * mWorldToCamera[0][2] + topLeft[1] * mWorldToCamera[1][2] + topLeft[2] * mWorldToCamera[2][2] + mWorldToCamera[3][2];
            w = topLeft[0] * mWorldToCamera[0][3] + topLeft[1] * mWorldToCamera[1][3] + topLeft[2] * mWorldToCamera[2][3] + mWorldToCamera[3][3];
            RealTopLeft = new vec3(a / w, b / w, c / w);

            a = bottomRight[0] * mWorldToCamera[0][0] + bottomRight[1] * mWorldToCamera[1][0] + bottomRight[2] * mWorldToCamera[2][0] + mWorldToCamera[3][0];
            b = bottomRight[0] * mWorldToCamera[0][1] + bottomRight[1] * mWorldToCamera[1][1] + bottomRight[2] * mWorldToCamera[2][1] + mWorldToCamera[3][1];
            c = bottomRight[0] * mWorldToCamera[0][2] + bottomRight[1] * mWorldToCamera[1][2] + bottomRight[2] * mWorldToCamera[2][2] + mWorldToCamera[3][2];
            w = bottomRight[0] * mWorldToCamera[0][3] + bottomRight[1] * mWorldToCamera[1][3] + bottomRight[2] * mWorldToCamera[2][3] + mWorldToCamera[3][3];
            RealBottomRight = new vec3(a / w, b / w, c / w);
        }
    }
}
